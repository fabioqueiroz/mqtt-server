using MQTT.CentralServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Entities.Scheduler
{
    public class SchedulerStatusInfo
    {
        public Guid Id { get; set; }
        public string SchedulerName { get; set; } = string.Empty;
        public ServiceStatus Status { get; set; }
        public DateTime DateOfLastUpdate { get; set; }

        public static SchedulerStatusInfo Create(string jobName)
        {
            return new SchedulerStatusInfo 
            { 
                SchedulerName = jobName,
                DateOfLastUpdate = DateTime.Now,
                Status = ServiceStatus.Initializing
            };
        }

        public void UpdateServiceStatus(ServiceStatus serviceStatus)
        {
            Status = serviceStatus;
            DateOfLastUpdate = DateTime.Now;
        }
    }
}
