using System.Security.Claims;
using System.Text;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class JwtTokenService(
    IConfiguration configuration,
    TokenValidationParameters tokenValidationParameters,
    ILogger<JwtTokenService> logger,
    IUnitOfWork unitOfWork)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly TokenValidationParameters _tokenValidationParameters = tokenValidationParameters;
    private readonly ILogger<JwtTokenService> _logger = logger;

    public async Task<(string token, DateTime expirationDate)> GenerateJwtTokenAsync(IEnumerable<Claim> claims)
    {
        try
        {
            var secretKey = _configuration["Jwt:SecretKey"] ??
                            throw new ApplicationException("JWT Secret Key is not configured");
            var issuer = _configuration["Jwt:ValidIssuer"] ??
                         throw new ApplicationException("JWT Issuer is not configured");
            var audience = _configuration["Jwt:ValidAudience"] ??
                           throw new ApplicationException("JWT Audience is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var claimsList = claims.ToList();
            var roleClaim = claimsList.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var expiryMinutes = roleClaim switch
            {
                "Admin" => int.Parse(_configuration["Jwt:AdminExpiryMinutes"]!),
                "User" => int.Parse(_configuration["Jwt:UserExpiryMinutes"]!),
                _ => int.Parse(_configuration["Jwt:UserExpiryMinutes"]!)
            };
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var fullClaims = AddStandardClaims(claimsList);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(fullClaims),
                NotBefore = DateTime.UtcNow
            };
            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            if (!(await tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters)).IsValid)
            {
                throw new SecurityTokenValidationException("Generated jwt Failed Validation!");
            }

            return (token, expiresAt);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate JWT Token");
            throw;
        }
    }
    private IEnumerable<Claim> AddStandardClaims(IEnumerable<Claim> claims)
    {
        var enhancedClaims = claims.ToList();

        if (enhancedClaims.All(c => c.Type != JwtRegisteredClaimNames.Jti))
        {
            enhancedClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        }

        if (enhancedClaims.All(c => c.Type != JwtRegisteredClaimNames.Iat))
        {
            enhancedClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, 
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()));
        }

        return enhancedClaims;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(
                int.Parse(configuration["Jwt:RefreshTokenExpiryHours"] ?? "5"))
        };
        // ToDo: Removing Old Tokens
        // *note*: Consider implementing a scheduled cleanUp Service 

        await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.CommitAsync();
        return refreshToken;
    }
    
}