using Microsoft.Extensions.DependencyInjection;
using MQTT.CentralServer.Data.Access;
using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Data.Access.Repositories;
using MQTT.CentralServer.Entities.Enums;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.Services.SchedulerStatus;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
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

            using var scope = _serviceProvider.CreateScope();
            var schedulerStatusService = scope.ServiceProvider.GetRequiredService<ISchedulerStatusService>();

            var status = await schedulerStatusService.CheckJobStatusAsync(jobName, context.CancellationToken);

            using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var _dbcontext = serviceScope.ServiceProvider.GetRequiredService<Context>();

                var repository = new SchedulerStatusRepository(_dbcontext);
                var service = new SchedulerStatusService(repository);

                if (status == (int)ServiceStatus.Initializing)
                {
                    //await schedulerStatusService.RecordSchedulerStatusAsync(jobName, context.CancellationToken, ServiceStatus.Started);
                    await service.RecordSchedulerStatusAsync(jobName, context.CancellationToken, ServiceStatus.Started);                   
                }
            }

            //if (status == (int)ServiceStatus.Initializing)
            //{
            //    await schedulerStatusService.RecordSchedulerStatusAsync(jobName, context.CancellationToken, ServiceStatus.Started); 
            //}

            // retry pattern https://dotnettutorials.net/lesson/retry-pattern-in-csharp/
        }
    }
}
