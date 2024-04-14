using Microsoft.Extensions.DependencyInjection;
using MQTT.CentralServer.Data.Access;
using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Data.Access.Migrations;
using MQTT.CentralServer.Data.Access.Repositories;
using MQTT.CentralServer.Entities.Enums;
using MQTT.CentralServer.Entities.Scheduler;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.Services.SchedulerStatus;
using MQTT.CentralServer.WorkerService.Server;
using MQTT.CentralServer.WorkerService.Strategies;
using Quartz;
using Quartz.Impl;
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
                var dbcontext = serviceScope.ServiceProvider.GetRequiredService<Context>();
                var schedulerRepository = new SchedulerStatusRepository(dbcontext);
                var mqttMessageRepository = new MqttMessageRepository(dbcontext);

                await CreateOrUpdateJobAsync(status, jobName, schedulerRepository, context.CancellationToken);

                if (status == (int)ServiceStatus.None || status == (int)ServiceStatus.Initializing || status == (int)ServiceStatus.Started)
                {
                    //var mqttServer = Server.MqttServer.Instance;
                    var mqttServer = new MqttServer(mqttMessageRepository);
                    await mqttServer.StartMqttServer();
                }
            }
        }

        private static async Task<int> GetJobStatus(IServiceProvider serviceProvider, string jobName, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var schedulerStatusService = scope.ServiceProvider.GetRequiredService<ISchedulerStatusService>();

            return await schedulerStatusService.CheckJobStatusAsync(jobName, cancellationToken);
        }

        private static async Task CreateOrUpdateJobAsync(int status, string jobName, SchedulerStatusRepository schedulerRepository, CancellationToken cancellationToken)
        {
            var statusRequest = new ChangeStatusRequest(status, jobName, schedulerRepository, cancellationToken);

            switch (status)
            {
                case (int)ServiceStatus.None:
                    await statusRequest.ChangeStatus(new InitializingJobStrategy(jobName));
                    break;
                case (int)ServiceStatus.Initializing:
                    await statusRequest.ChangeStatus(new StartedJobStrategy(jobName));
                    break;
                case (int)ServiceStatus.Started:
                    break;
                case (int)ServiceStatus.Closing:
                    await statusRequest.ChangeStatus(new EndedJobStrategy(jobName));
                    break;
                default:
                    throw new InvalidOperationException("Invalid status.");
            }
        }
    }
}
