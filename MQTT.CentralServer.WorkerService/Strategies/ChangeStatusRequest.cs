using MQTT.CentralServer.Data.Access.Repositories;
using MQTT.CentralServer.WorkerService.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Strategies
{
    public class ChangeStatusRequest
    {
        public int Status { get; init; }
        public string JobName { get; init; } = string.Empty;
        public SchedulerStatusRepository SchedulerRepository { get; }
        public CancellationToken CancellationToken { get; init; } = CancellationToken.None;

        public ChangeStatusRequest(int status, string jobName, SchedulerStatusRepository schedulerRepository, CancellationToken cancellationToken)
        {
            Status = status;
            JobName = jobName;
            SchedulerRepository = schedulerRepository;
            CancellationToken = cancellationToken;
        }

        public async Task ChangeStatus(IStatusStrategy statusStrategy)
        {
            await statusStrategy.UpdateServiceStatus(SchedulerRepository, CancellationToken);
        }
    }
}
