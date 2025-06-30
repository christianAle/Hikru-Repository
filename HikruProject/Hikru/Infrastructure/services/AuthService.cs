using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HikruCodeChallenge.Infrastructure.Services;

public interface IAuthService
{
    string GenerateToken(string userId, string email);
    bool ValidateApiKey(string apiKey);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecret;
    private readonly string _validApiKey;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSecret = _configuration["JWT:Secret"] ?? "YourSecretKeyHere123456789012345678901234567890";
        _validApiKey = _configuration["Authentication:ApiKey"] ?? "hikru-api-key-2025";
    }

    public string GenerateToken(string userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"] ?? "HikruAPI",
            audience: _configuration["JWT:Audience"] ?? "HikruClients",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateApiKey(string apiKey)
    {
        return !string.IsNullOrEmpty(apiKey) && apiKey == _validApiKey;
    }
}
