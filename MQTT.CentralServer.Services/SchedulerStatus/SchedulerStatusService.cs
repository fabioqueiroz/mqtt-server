using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Enums;
using MQTT.CentralServer.Entities.Scheduler;
using MQTT.CentralServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Services.SchedulerStatus
{
    public class SchedulerStatusService : ISchedulerStatusService
    {
        private readonly ISchedulerStatusRepository _schedulerStatusRepository;
        public SchedulerStatusService(ISchedulerStatusRepository schedulerStatusRepository)
        {
            _schedulerStatusRepository = schedulerStatusRepository;
        }

        public async Task<int> CheckJobStatusAsync(string jobName, CancellationToken cancellationToken)
        {
            return await _schedulerStatusRepository.CheckJobStatusAsync(jobName, cancellationToken);
        }

        public async Task RecordSchedulerStatusAsync(string jobName, CancellationToken cancellationToken, ServiceStatus serviceStatus = ServiceStatus.Initializing)
        {
            var schedulerStatusInfo = new SchedulerStatusInfo { SchedulerName = jobName, DateOfLastUpdate = DateTime.Now, Status = serviceStatus };

            await _schedulerStatusRepository.RecordSchedulerStatusAsync(schedulerStatusInfo, cancellationToken);
        }
    }
}
