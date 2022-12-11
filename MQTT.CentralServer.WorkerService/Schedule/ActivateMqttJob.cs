using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Schedule
{
    [DisallowConcurrentExecution]
    public class ActivateMqttJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivateMqttJob> _logger;
        public ActivateMqttJob(IServiceProvider serviceProvider, ILogger<ActivateMqttJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("***** ActivateMqttJob is executing. ********");

            // TODO: create new table to store/check the status - started, starting, ended
            // retry pattern https://dotnettutorials.net/lesson/retry-pattern-in-csharp/

            return Task.CompletedTask;
        }
    }
}
