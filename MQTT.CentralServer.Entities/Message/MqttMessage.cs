using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Entities.Message
{
    public class MqttMessage
    {
        public Guid Id { get; init; }
        public string Topic { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string ClientId { get; init; } = string.Empty;
        public DateTime DateReceived { get; init; }

        public static MqttMessage Create(string topic, string message, string clientId)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            return new MqttMessage
            {
                Topic = topic,
                Message = message,
                ClientId = clientId,
                DateReceived = DateTime.UtcNow
            };
        }
    }
}
