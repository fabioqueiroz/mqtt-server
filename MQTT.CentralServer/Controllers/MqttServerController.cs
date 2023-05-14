using Microsoft.AspNetCore.Mvc;
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
        private readonly IIdentityServerService _identityServerService;
        public MqttServerController(
            ILogger<MqttServerController> logger, 
            IMqttJobService mqttJobService, 
            ISchedulerStatusService schedulerStatusService, 
            IIdentityServerService identityServerService)
        {
            _logger = logger;
            _mqttJobService = mqttJobService;
            _schedulerStatusService = schedulerStatusService;
            _identityServerService = identityServerService;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetToken()
        {
            return Ok(await _identityServerService.GetTokenAsync());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> StartJobService([FromBody]string token)
        {
            var isAuthenticated = await _identityServerService.AuthenticateWithTokenAsync(token);
            if (!isAuthenticated)
            {
                return BadRequest($"Unable to authenticate with token {token}");
            }

            await _mqttJobService.StartAsync(new CancellationToken());
            return Ok("Job service started");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> JobShutDown()
        {
            await _mqttJobService.StopAsync(new CancellationToken());
            return Ok("Job service shut down");
        }

        [HttpDelete]
        [Route("[action]")]
        public async void DeleteJob(string jobName)
        {
            await _schedulerStatusService.UpdateJobStatusToClosingByNameAsync(jobName, new CancellationToken());
        }
    }
}
