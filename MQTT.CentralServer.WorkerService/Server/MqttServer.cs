using MQTT.CentralServer.Data.Access.Interfaces;
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
        private readonly IMqttMessageRepository _mqttMessageRepository;
        public MqttServer(IMqttMessageRepository mqttMessageRepository)
        {
            _mqttMessageRepository = mqttMessageRepository;
        }

        //public MqttServer()
        //{            
        //}

        //public static MqttServer Instance
        //{
        //    get
        //    {
        //        _instance ??= new MqttServer();
        //        return _instance;
        //    }
        //}

        public async Task StartMqttServer()
        {
            var mqttServerGenerator = new MqttServerGenerator(_mqttMessageRepository);
            await mqttServerGenerator.Generate();
        }
    }
}
