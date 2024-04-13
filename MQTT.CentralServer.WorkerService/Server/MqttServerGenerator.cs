using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Server;
using MQTTnet;
using MQTT.CentralServer.Entities.Message;
using MQTT.CentralServer.Data.Access.Interfaces;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.Services.Messages;
using MQTT.CentralServer.Data.Access.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MQTT.CentralServer.Data.Access;
using Microsoft.EntityFrameworkCore;

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

        async Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            var message = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine($"MqttServer_InterceptingPublishAsync - {arg.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}");

            var receivedMessage = MqttMessage.Create(topic: arg.ApplicationMessage.Topic, message: message, clientId: arg.ClientId);    
            await AddReceivedMessageAsync(receivedMessage);
            //return Task.CompletedTask;
        }

        Task MqttServer_LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs arg)
        {
            Console.WriteLine("MqttServer_LoadingRetainedMessageAsync");
            return Task.CompletedTask;
        }

        private async Task AddReceivedMessageAsync(MqttMessage receivedMessage, CancellationToken cancellationToken = default)
        {
            var serviceProvider = new ServiceCollection()
                    .AddDbContext<Context>(options =>
                    options.UseSqlServer("Data Source=UKfqueMP26AKM7\\SQLEXPRESS;Initial Catalog=Quartz_Migration_1;Integrated Security=False;User Id=sa;Password=Fabio1980;MultipleActiveResultSets=True;TrustServerCertificate=True",
                    opts =>
                    {
                        opts.EnableRetryOnFailure((int)TimeSpan.FromSeconds(5).TotalSeconds);
                        opts.CommandTimeout((int)TimeSpan.FromMinutes(2).TotalSeconds);
                    }))
                    .BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var dbcontext = serviceProvider.GetRequiredService<Context>();
            var messageRepository = new MqttMessageRepository(dbcontext);
            await messageRepository.AddAsync(receivedMessage, cancellationToken);
        }
    }
}
