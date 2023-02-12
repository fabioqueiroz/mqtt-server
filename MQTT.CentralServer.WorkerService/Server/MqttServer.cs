using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.WorkerService.Server
{
    public class MqttServer
    {
        private static MqttServer? _instance;
        private MqttServer()
        {

        }

        public static MqttServer Instance
        {
            get
            {
                _instance ??= new MqttServer();
                return _instance;
            }
        }

        public async Task StartMqttServer()
        {
            var mqttServerGenerator = new MqttServerGenerator();
            await mqttServerGenerator.Generate();
        }
    }
}
