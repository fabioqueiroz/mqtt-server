using MQTT.CentralServer.Entities.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Data.Access.Interfaces
{
    public interface ISchedulerStatusRepository
    {
        Task RecordSchedulerStatusAsync(SchedulerStatusInfo schedulerStatus, CancellationToken cancellationToken);
        Task<int> CheckJobStatusAsync(string jobName, CancellationToken cancellationToken);
    }
}
