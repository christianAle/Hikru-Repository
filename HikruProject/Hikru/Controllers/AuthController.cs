using HikruCodeChallenge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace HikruCodeChallenge.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    /// <summary>
    /// Get the valid API key for testing
    /// </summary>
    /// <returns>API key information</returns>
    [HttpGet("api-key")]
    [ProducesResponseType(typeof(ApiKeyResponse), 200)]
    public ActionResult<ApiKeyResponse> GetApiKey()
    {

        // This could be in a separete service
        var key = _configuration["Authentication:ApiKey"];

        return Ok(new ApiKeyResponse 
        { 
            ApiKey = key,
            Message = "Use this API key in the X-API-Key header or Authorization header as 'Bearer {apiKey}'"
        });
    }
}

public class ApiKeyResponse
{
    public string ApiKey { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
