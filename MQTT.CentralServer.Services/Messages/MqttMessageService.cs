using Microsoft.Extensions.Logging;
using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Entities.Message;
using MQTT.CentralServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Services.Messages
{
    public class MqttMessageService : IMqttMessageService
    {
        //private readonly ILogger<MqttMessageService> _logger;
        private readonly IMqttMessageRepository _messageRepository;
        //public MqttMessageService(ILogger<MqttMessageService> logger, IMqttMessageRepository messageRepository)
        //{
        //    _logger = logger;
        //    _messageRepository = messageRepository;
        //}

        public MqttMessageService(IMqttMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task AddMessageAsync(MqttMessage message, CancellationToken cancellationToken = default)
        {
            //_logger.LogInformation($"New message received with topic {message.Topic} for client {message.ClientId} received.");

            await _messageRepository.AddAsync(message, cancellationToken);
        }
    }
}
