using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Strategies
{
    public abstract class BaseStatusStrategy
    {
        public string JobName { get; init; } = string.Empty;
        public BaseStatusStrategy(string jobName)
        {
            JobName = jobName;
        }
    }
}
