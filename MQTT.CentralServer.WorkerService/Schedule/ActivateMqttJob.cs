using Microsoft.Extensions.DependencyInjection;
using MQTT.CentralServer.Data.Access;
using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Data.Access.Repositories;
using MQTT.CentralServer.Entities.Enums;
using MQTT.CentralServer.Entities.Scheduler;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.Services.SchedulerStatus;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Schedule
{
    [DisallowConcurrentExecution]
    public class ActivateMqttJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivateMqttJob> _logger;

        public ActivateMqttJob(IServiceProvider serviceProvider, ILogger<ActivateMqttJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("***** ActivateMqttJob is executing. ********");

            var jobName = context.JobDetail.Key.Name;
            var status = await GetJobStatus(_serviceProvider, jobName, context.CancellationToken);

            using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var _dbcontext = serviceScope.ServiceProvider.GetRequiredService<Context>();
                var schedulerRepository = new SchedulerStatusRepository(_dbcontext);

                if (status == (int)ServiceStatus.None)
                {
                    var schedulerStatusInfo = SchedulerStatusInfo.Create(jobName);
                    await schedulerRepository.RecordSchedulerStatusAsync(schedulerStatusInfo, context.CancellationToken);
                }

                if (status == (int)ServiceStatus.Initializing)
                {
                    var schedulerStatusInfo = await schedulerRepository.GetJobStatusByNameAsync(jobName, context.CancellationToken);
                    schedulerStatusInfo.UpdateServiceStatus(ServiceStatus.Started);
                    await schedulerRepository.UpdateSchedulerStatusAsync(schedulerStatusInfo, context.CancellationToken);
                }
            }

            // retry pattern https://dotnettutorials.net/lesson/retry-pattern-in-csharp/
        }

        private static async Task<int> GetJobStatus(IServiceProvider serviceProvider, string jobName, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var schedulerStatusService = scope.ServiceProvider.GetRequiredService<ISchedulerStatusService>();

            return await schedulerStatusService.CheckJobStatusAsync(jobName, cancellationToken);
        }
    }
}
