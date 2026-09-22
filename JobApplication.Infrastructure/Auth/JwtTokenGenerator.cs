using JobApplication.Application.Interfaces.Auth;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _s;
    public JwtTokenGenerator(IOptions<JwtSettings> options) => _s = options.Value;

    public (string Token, DateTime ExpiresAt) Generate(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("profileId", user.ProfileId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_s.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_s.ExpiryMinutes);

        var token = new JwtSecurityToken(_s.Issuer, _s.Audience, claims, expires: expires, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}