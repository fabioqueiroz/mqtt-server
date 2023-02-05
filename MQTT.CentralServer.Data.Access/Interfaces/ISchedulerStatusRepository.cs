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
        Task<int> CheckJobStatusAsync(string jobName, CancellationToken cancellationToken);
        Task<SchedulerStatusInfo> GetJobStatusByNameAsync(string jobName, CancellationToken cancellationToken);
        Task RecordSchedulerStatusAsync(SchedulerStatusInfo schedulerStatus, CancellationToken cancellationToken);
        Task UpdateSchedulerStatusAsync(SchedulerStatusInfo schedulerStatus, CancellationToken cancellationToken);
        Task UpdateJobStatusToClosingByNameAsync(string jobName, CancellationToken cancellationToken);
        Task DeleteJobByNameAsync(string jobName, CancellationToken cancellationToken);
    }
}
