using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTT.CentralServer.Entities.Options;
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
        private readonly IOptions<ApiConfigs> _options;
        public IdentityServerService(HttpClient httpClient, ILogger<IdentityServerService> logger, IOptions<ApiConfigs> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<string> GetTokenAsync()
        {
            var discoDocument = await _httpClient.GetDiscoveryDocumentAsync(_options.Value.IdentityProviderApi.Uri);
            if (discoDocument.IsError)
            {
                _logger.LogError(discoDocument.Error);
                throw new InvalidDataException(discoDocument.Error);
            }

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                //// In-memory scope
                //Address = discoDocument.TokenEndpoint,
                //ClientId = _options.Value.IdentityServerApi.ClientId,
                //ClientSecret = _options.Value.IdentityServerApi.ClientSecret,
                //Scope = _options.Value.IdentityServerApi.Scope
                Address = discoDocument.TokenEndpoint,
                ClientId = _options.Value.IdentityProviderApi.ClientId,
                ClientSecret = _options.Value.IdentityProviderApi.ClientSecret,
                Scope = _options.Value.IdentityProviderApi.Scope
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

            //var response = await _httpClient.GetAsync("https://localhost:7031/Identity");
            var response = await _httpClient.GetAsync(_options.Value.IdentityServerApi.Uri);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Unable to authenticate. Status response: {response.StatusCode}");
                //throw new InvalidDataException($"{response.StatusCode}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
