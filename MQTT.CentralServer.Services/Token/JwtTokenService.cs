using Microsoft.Extensions.Logging;
using MQTT.CentralServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MQTT.CentralServer.Entities.Options;

namespace MQTT.CentralServer.Services.Token
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly IOptions<ApiConfigs> _options;

        public JwtTokenService(HttpClient httpClient, ILogger<JwtTokenService> logger, IOptions<ApiConfigs> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<bool> ForwardTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var isSuccessStatusCode = false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync(_options.Value.BackChannelApi.TokenEndpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    isSuccessStatusCode = true;
                    _logger.LogInformation($"Response: {responseBody}");

                }
                else
                {
                    _logger.LogError($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception: {ex.Message}");
            }

            return isSuccessStatusCode;
        }
    }
}
