using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Scheduler;
using MQTT.CentralServer.WorkerService.Strategies.Interfaces;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Strategies
{
    public class InitializingStrategy : IStatusStrategy
    {
        public string JobName { get; init; } = string.Empty;

        public InitializingStrategy(string jobName)
        {
            JobName = jobName;
        }

        public async Task UpdateServiceStatus(ISchedulerStatusRepository schedulerRepository, CancellationToken cancellationToken)
        {
            var schedulerStatusInfo = SchedulerStatusInfo.Create(JobName);
            await schedulerRepository.RecordSchedulerStatusAsync(schedulerStatusInfo, cancellationToken);
        }
    }
}
