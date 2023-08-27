using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Entities.Options
{
    public class ApiConfigs
    {
        public IdentityServerApi IdentityServerApi { get; init; } = new();
        public IdentityProviderApi IdentityProviderApi { get; init; } = new();
        public BackChannelApi BackChannelApi { get; init; } = new();
    }

    public class IdentityServerApi
    {
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
        public string Scope { get; init; } = string.Empty;
        public string Uri { get; init; } = string.Empty;
    }

    public class IdentityProviderApi
    {
        public string Uri { get; init; } = string.Empty;
        public string TokenUri { get; init; } = string.Empty;
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
        public string Scope { get; init; } = string.Empty;
    }

    public class BackChannelApi
    {
        public string Uri { get; init; } = string.Empty;
        public string TokenEndpoint { get; init; } = string.Empty;
    }
}
