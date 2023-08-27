using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MQTT.CentralServer.Services.Interfaces;
using System.Net.Http.Headers;

namespace MQTT.ClientReactApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JwtTokenController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        public JwtTokenController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTokenAsync()
        {
            var token = await Request.HttpContext.GetUserAccessTokenAsync();

            var result = await _jwtTokenService.ForwardTokenAsync(token.AccessToken!);

            if (result == false)
            {
                return BadRequest("Unable to forward token");
            }

            return Ok("Back channel endpoint called to get token");
        }
    }
}
