using MQTT.CentralServer.Entities.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Data.Access.Interfaces
{
    public interface IMqttMessageRepository
    {
        Task AddAsync(MqttMessage message, CancellationToken cancellationToken = default);
    }
}
