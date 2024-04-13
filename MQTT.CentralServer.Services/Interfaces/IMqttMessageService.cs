using MQTT.CentralServer.Entities.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Services.Interfaces
{
    public interface IMqttMessageService
    {
        Task AddMessageAsync(MqttMessage message, CancellationToken cancellationToken = default);
    }
}
