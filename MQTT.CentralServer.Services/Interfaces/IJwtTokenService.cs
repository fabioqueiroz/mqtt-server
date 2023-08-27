using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Services.Interfaces
{
    public interface IJwtTokenService
    {
        Task<bool> ForwardTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
