using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Data.Access.Repositories
{
    public class SchedulerStatusRepository : BaseRepository<SchedulerStatusInfo>, ISchedulerStatusRepository
    {
        public SchedulerStatusRepository(Context context) : base(context)
        {

        }

        public async Task<int> CheckJobStatusAsync(string jobName, CancellationToken cancellationToken)
        {
            var jobEntry = await GetSingleAsync<SchedulerStatusInfo>(x => x.SchedulerName.Equals(jobName));
            return jobEntry != null ? (int)jobEntry.Status : 0;
        }

        public async Task RecordSchedulerStatusAsync(SchedulerStatusInfo schedulerStatus, CancellationToken cancellationToken)
        {
            await _context.AddAsync(schedulerStatus, cancellationToken);
            await CommitAsync();
        }
    }
}
