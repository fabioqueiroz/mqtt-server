using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Strategies
{
    public abstract class BaseJobStrategy
    {
        public string JobName { get; init; } = string.Empty;
        public BaseJobStrategy(string jobName)
        {
            JobName = jobName;
        }
    }
}
