using Microsoft.EntityFrameworkCore;
using MQTT.CentralServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Services.Interfaces
{
    public interface ISchedulerStatusService
    {
        Task RecordSchedulerStatusAsync(string jobName, CancellationToken cancellationToken, ServiceStatus serviceStatus);
        Task<int> CheckJobStatusAsync(string jobName, CancellationToken cancellationToken);
    }
}
