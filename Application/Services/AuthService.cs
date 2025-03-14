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
        return await CheckExistence(registerDto.Username, registerDto.Email);
    }
    
    private async Task<Result> CheckExistence(string username, string email)
    {
        var userExists = await userManager.FindByNameAsync(username);
        var result = new Result();
        if (userExists != null)
        {
            result.Message = "Username already exists!";
            return result;
        }
        userExists = await userManager.FindByEmailAsync(email);
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
        try
        {
            var token = await unitOfWork.RefreshTokenRepository.GetAsync(t => t.Token == refreshToken);
            if (token == null)
            {
                throw new SecurityTokenException("Invalid Refresh Token");
            }

            if (!token.IsActive)
            {
                throw new SecurityTokenException("Deactivated Refresh Token");
            }

            await RevokeTokenAsync(refreshToken, "Replaced by a new refresh token");
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
        catch (ApplicationException ex)
        {
            return new AuthResult()
            {
                Message = ex.Message
            };
        }
        catch (SecurityTokenException ex)
        {
            return new AuthResult()
            {
                Message = ex.Message
            };
        }
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
        var success = await unitOfWork.CommitAsync() >= 1;
        return success;
    }
    public async Task<AuthResult> LoginAsync(LoginDto loginDto)
    {
        try
        {
            AppUser? user;
            var result = new AuthResult();
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
        catch (ApplicationException ex)
        {
            return new AuthResult()
            {
                Message = ex.Message
            };
        }
        catch (SecurityTokenException ex)
        {
            return new AuthResult()
            {
                Message = ex.Message
            };
        }
    }

    public async Task<UserDto> GetUserInfoAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new UserDto();
        }

        var result = new UserDto()
        {
            UserName = user.UserName,
            Email = user.Email,
            ImageUrl = user.ImageUrl,
            IsVerified = user.EmailConfirmed
        };
        return result;
    }
    
    public async Task<AuthResult> LoginWithExternalProviderAsync(int userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new AuthResult { Message = "User not found" };
            }
            
            var claims = await GenerateUserClaimsAsync(user);
            var (accessToken, expiresAt) = await jwtTokenService.GenerateJwtTokenAsync(claims);
            
            // Start a transaction for refresh token generation and storage
            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var refreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user.Id);
                // Store the refresh token in the database
                transaction.Commit();
                
                return new AuthResult
                {
                    Success = true,
                    AccessToken = accessToken,
                    AccessTokenExpiration = expiresAt,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new AuthResult { Message = $"Failed to generate refresh token: {ex.Message}" };
            }
        }
        catch (ApplicationException ex)
        {
            return new AuthResult { Message = ex.Message };
        }
        catch (SecurityTokenException ex)
        {
            return new AuthResult { Message = ex.Message };
        }
    }
    
    public async Task<AuthResult> HandleGoogleAuthCallbackAsync(string googleId, string email, string name)
    {
        if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
        {
            return new AuthResult { Message = "Email or Google ID not available" };
        }

        try
        {
            var username = email.Split('@')[0]; // Use part of email as username
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var existenceResult = await CheckExistence(username, email);
                if (!existenceResult.Success && existenceResult.Message == "Username already exists!")
                {
                    username = $"{username}{new Random().Next(1000, 9999)}"; // to make it unique
                }
                
                user = new AppUser
                {
                    UserName = username,
                    Email = email,
                    GoogleId = googleId,
                    IsExternalAccount = true,
                    EmailConfirmed = true
                };

                using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        transaction.Rollback();
                        return new AuthResult { Message = "Failed to create user from Google account" };
                    }

                    var roleResult = await userManager.AddToRoleAsync(user, "User");
                    if (!roleResult.Succeeded)
                    {
                        transaction.Rollback();
                        return new AuthResult { Message = "Failed to assign role to new user" };
                    }
                    
                    await unitOfWork.CommitAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new AuthResult { Message = ex.Message };
                }
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = googleId;
                user.IsExternalAccount = true;
                
                using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        transaction.Rollback();
                        return new AuthResult { Message = "Failed to link Google account to existing user" };
                    }
                    await unitOfWork.CommitAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new AuthResult { Message = ex.Message };
                }
            }

            return await LoginWithExternalProviderAsync(user.Id);
        }
        catch (Exception ex)
        {
            return new AuthResult { Message = ex.Message };
        }
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
        
        if (user.IsExternalAccount && !string.IsNullOrEmpty(user.GoogleId))
        {
            claims.Add(new Claim("ExternalProvider", "Google"));
            claims.Add(new Claim("GoogleId", user.GoogleId));
        }

        return claims;
    }
}