using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Enums;
using MQTT.CentralServer.WorkerService.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Strategies
{
    public class EndedJobStrategy : BaseJobStrategy, IStatusStrategy
    {
        public EndedJobStrategy(string jobName) : base(jobName)
        {

        }

        public async Task UpdateServiceStatus(ISchedulerStatusRepository schedulerRepository, CancellationToken cancellationToken)
        {
            var schedulerStatusInfo = await schedulerRepository.GetJobStatusByNameAsync(JobName, cancellationToken);
            schedulerStatusInfo.UpdateServiceStatus(ServiceStatus.Ended);
            await schedulerRepository.UpdateSchedulerStatusAsync(schedulerStatusInfo, cancellationToken);
        }
    }
}
