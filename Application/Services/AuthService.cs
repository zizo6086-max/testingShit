using System.Security.Claims;
using System.Security.Cryptography;
using Application.DTOs;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IConfiguration configuration,
    JwtTokenService jwtTokenService)
{

    private async Task<Result> CheckExistence(RegisterDto registerDto)
    {
        var userExists = await userManager.FindByNameAsync(registerDto.Username);
        var result = new Result();
        if (userExists != null)
        {
            result.Message = "Username already exists!";
            return result;
        }
        userExists = await userManager.FindByEmailAsync(registerDto.Email);
        if (userExists != null)
        {
            result.Message = "Email already exists!";
            return result;
        }
        result.Success = true;
        return result;
    }
    public async Task<Result> RegisterAsync(RegisterDto registerDto,string role)
    {
        Result result = await CheckExistence(registerDto);
        if (!result.Success)
        {
            return result;
        }

        var newUser = new AppUser()
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
        };
        using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var creationResult = await userManager.CreateAsync(newUser, registerDto.Password);
            if (!creationResult.Succeeded)
            {
                transaction.Rollback();
                result.Success = false;
                result.Message = "Failed to create new user!";
            }

            if (!(await userManager.AddToRoleAsync(newUser, role)).Succeeded)
            {
                transaction.Rollback();
                result.Success = false;
                result.Message = "Failed to assign role to new user!";
                return result;
            }

            await unitOfWork.CommitAsync();
            transaction.Commit();
            result.Message = "Successfully registered new user!";
            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            result.Success = false;
            result.Message = ex.Message;
            result.Data = ex;
            return result;
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var token = await unitOfWork.RefreshTokenRepository.GetAsync(t=> t.Token == refreshToken);
        if (token == null)
        {
            throw new SecurityTokenException("Invalid Refresh Token");
        }

        if (!token.IsActive)
        {
            throw new SecurityTokenException("Deactivated Refresh Token");
        }
        await RevokeTokenAsync(refreshToken,"Replaced by a new refresh token"); 
        var user = await userManager.FindByIdAsync(token.UserId.ToString());
        if (user == null)
        {
            throw new SecurityTokenException("User is not found");
        }

        var newRefreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user.Id);
        var claims = await GenerateUserClaimsAsync(user);
        var (accessToken, expiresAt) = await jwtTokenService.GenerateJwtTokenAsync(claims);
        return new AuthResult()
        {
            Success = true,
            Message = "Successfully Refreshed Token",
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiration = expiresAt,
            RefreshTokenExpiration = newRefreshToken.ExpiresAt,
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, string? revokeReason = null)
    {
        var token = await unitOfWork.RefreshTokenRepository.GetAsync(t=> t.Token == refreshToken);
        if (token == null)
        {
            throw new SecurityTokenException("Invalid Refresh Token");
        }

        if (!token.IsActive)
        {
            throw new SecurityTokenException("Deactivated Refresh Token");
        }

        token.IsRevoked = true;
        token.RevokeReason = revokeReason;
        await unitOfWork.RefreshTokenRepository.UpdateAsync(token);
        return await unitOfWork.CommitAsync() < 1;
    }
    public async Task<AuthResult> LoginAsync(LoginDto loginDto)
    {
        AppUser? user;
        AuthResult result = new AuthResult();
        if (loginDto.EmailOrUsername.Contains('@'))
            user = await userManager.FindByEmailAsync(loginDto.EmailOrUsername);
        else
            user = await userManager.FindByNameAsync(loginDto.EmailOrUsername);
        if (user == null || !(await userManager.CheckPasswordAsync(user, loginDto.Password)))
        {
            result.Message = "Invalid Credentials!";
            return result;
        }
        var claims = await GenerateUserClaimsAsync(user);
        var (accessToken, expiresAt) = await jwtTokenService.GenerateJwtTokenAsync(claims);
        var refreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user.Id);
        result.Success = true;
        result.AccessToken = accessToken;
        result.AccessTokenExpiration = expiresAt;
        result.RefreshToken = refreshToken.Token;
        result.RefreshTokenExpiration = expiresAt;
        return result;
    }
    private async Task<List<Claim>> GenerateUserClaimsAsync(AppUser user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.UserName!),
            new (ClaimTypes.Email, user.Email!)
        };

        // Add roles
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add custom claims
        var userClaims = await userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        return claims;
    }
}