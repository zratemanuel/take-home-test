using Fundo.Applications.Domain.Interfaces;
using Fundo.Applications.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApiSecurity.Controllers;

[Route("api/security")]
[ApiController]
public class AuthenticateController : Controller
{
    private readonly ILogger<AuthenticateController> _logger;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticateController(ILogger<AuthenticateController> logger, IJwtTokenService jwtTokenService)
    {
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// EndPoint used for authentication via previously registered user and password
    /// </summary>
    /// <param name="RequestLogin">Object containing user and password requested for authentication</param>
    /// <returns>
    /// returns an object containing the token
    //  
    //      "email": "test@test.com",
    //      "token": "85793408574935.55967850.69678"
    //  
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> Login([FromBody] RequestLogin requestLogin)
    {
        try
        {
            string token = await _jwtTokenService.LoginUserAsync(requestLogin);

            return String.IsNullOrEmpty(token) ?
                StatusCode(StatusCodes.Status401Unauthorized, "User or Password incorrect.") :
                Ok(new ResponseLogin { Email = requestLogin.User, Token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }
}
