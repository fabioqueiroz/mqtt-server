﻿using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Enums;
using MQTT.CentralServer.WorkerService.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Strategies
{
    public class StartedStrategy : IStatusStrategy
    {
        public string JobName { get; init; } = string.Empty;

        public StartedStrategy(string jobName)
        {
            JobName = jobName;
        }

        public async Task UpdateServiceStatus(ISchedulerStatusRepository schedulerRepository, CancellationToken cancellationToken)
        {
            var schedulerStatusInfo = await schedulerRepository.GetJobStatusByNameAsync(JobName, cancellationToken);
            schedulerStatusInfo.UpdateServiceStatus(ServiceStatus.Started);
            await schedulerRepository.UpdateSchedulerStatusAsync(schedulerStatusInfo, cancellationToken);
        }
    }
}