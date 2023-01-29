using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Entities.Enums
{
    public enum ServiceStatus
    {
        Initializing = 1,
        Started = 2,
        Closing = 3,
        Ended = 4
    }
}
