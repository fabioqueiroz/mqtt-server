using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Server;
using MQTTnet;
using MQTT.CentralServer.Entities.Scheduler;

namespace MQTT.CentralServer.WorkerService.Server
{
    public class MqttServerGenerator
    {
        public async Task Generate()
        {
            Console.WriteLine("MQTT Server");

            //// Start a MQTT server.
            var mqttFactory = new MqttFactory();

            // The port for the default endpoint is 1883.
            // The default endpoint is NOT encrypted!
            // Use the builder classes where possible.
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .Build();

            using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
            {
                await mqttServer.StartAsync();

                mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
                mqttServer.ApplicationMessageNotConsumedAsync += MqttServer_ApplicationMessageNotConsumedAsync;
                mqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;
                mqttServer.LoadingRetainedMessageAsync += MqttServer_LoadingRetainedMessageAsync;


                static Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
                {
                    Console.WriteLine("Inside event handler");
                    return Task.CompletedTask;
                }

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();

                // Stop and dispose the MQTT server if it is no longer needed!
                await mqttServer.StopAsync();
            }
        }

        Task MqttServer_ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
        {
            Console.WriteLine($"MqttServer_ApplicationMessageNotConsumedAsync - {arg.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");
            return Task.CompletedTask;
        }

        Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            var message = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine($"MqttServer_InterceptingPublishAsync - {arg.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");

            var receivedMessage = MqttMessage.Create(topic: arg.ApplicationMessage.Topic, message: message, clientId: arg.ClientId);

            // TODO: create service

            return Task.CompletedTask;
        }

        Task MqttServer_LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs arg)
        {
            Console.WriteLine("MqttServer_LoadingRetainedMessageAsync");
            return Task.CompletedTask;
        }
    }
}
