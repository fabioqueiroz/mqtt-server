using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using MQTT.CentralServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace MQTT.CentralServer.Services.SchedulerStatus
{
    public class IdentityServerService : IIdentityServerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IdentityServerService> _logger;

        public IdentityServerService(HttpClient httpClient, ILogger<IdentityServerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync()
        {
            var discoDocument = await _httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (discoDocument.IsError)
            {
                _logger.LogError(discoDocument.Error);
                throw new InvalidDataException(discoDocument.Error);
            }

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoDocument.TokenEndpoint,
                ClientId = "m2m.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "scope1"
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError(tokenResponse.Error);
                throw new InvalidDataException(tokenResponse.Error);
            }

            _logger.LogInformation($"Access token: {tokenResponse.AccessToken}");

            return tokenResponse.AccessToken;
        }

        public async Task<bool> AuthenticateWithTokenAsync(string token)
        {
            _httpClient.SetBearerToken(token);

            var response = await _httpClient.GetAsync("https://localhost:7031/Identity");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Unable to authenticate. Status response: {response.StatusCode}");
                //throw new InvalidDataException($"{response.StatusCode}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
