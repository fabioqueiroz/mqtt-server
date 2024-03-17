using Microsoft.AspNetCore.SignalR;

namespace MQTT.CentralServer.Api.SignalR
{
    public class MQTTMessageHub : Hub
    {
        public async Task SendMessageAsync(string user, string message)
        {
            await Clients.All.SendAsync("Received message", user, message);
        }
    }
}
