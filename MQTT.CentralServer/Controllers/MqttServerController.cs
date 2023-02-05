using Microsoft.AspNetCore.Mvc;
using MQTT.CentralServer.Controllers;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.WorkerService.Services.Interfaces;

namespace MQTT.CentralServer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MqttServerController : ControllerBase
    {
        private readonly ILogger<MqttServerController> _logger;
        private readonly IMqttJobService _mqttJobService;
        private readonly ISchedulerStatusService _schedulerStatusService;
        public MqttServerController(ILogger<MqttServerController> logger, IMqttJobService mqttJobService, ISchedulerStatusService schedulerStatusService)
        {
            _logger = logger;
            _mqttJobService = mqttJobService;
            _schedulerStatusService = schedulerStatusService;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }

        [HttpPost]
        [Route("[action]")]
        public async void StartJobService()
        {
            await _mqttJobService.StartAsync(new CancellationToken());
        }

        [HttpPost]
        [Route("[action]")]
        public async void JobShutDown()
        {
            await _mqttJobService.StopAsync(new CancellationToken());
        }

        [HttpDelete]
        [Route("[action]")]
        public async void DeleteJob(string jobName)
        {
            await _schedulerStatusService.UpdateJobStatusToClosingByNameAsync(jobName, new CancellationToken());
        }
    }
}
