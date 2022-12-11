using MQTT.CentralServer.Api.Controllers;
using MQTT.CentralServer.Api.Services.Interfaces;

namespace MQTT.CentralServer.Api.Services
{
    public class ActivateMqttService : IActivateMqttService
    {
        private readonly ILogger<ActivateMqttService> _logger;
        public ActivateMqttService(ILogger<ActivateMqttService> logger)
        {
            _logger = logger;
        }
    }
}
