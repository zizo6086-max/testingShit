using Application.DTOs;
using Domain.Models.Auth;
using FluentEmail.Core;
using Infrastructure.EmailTemplates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class EmailService(IFluentEmail fluentEmail, IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
{
    private readonly IFluentEmail _fluentEmail = fluentEmail;
    private readonly IConfiguration _configuration = configuration;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    public async Task<Result> SendEmailVerificationAsync(EmailVerificationRequest model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.email);
            if (user == null)
            {
                return new Result()
                {
                    Message = "User not found"
                };
            }

            if (user.EmailConfirmed)
            {
                return new Result()
                {
                    Success = true,
                    Message = "Email already verified"
                };
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7282/";
            var verificationLink =
                $"{baseUrl}api/auth/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            var emailBody = EmailVerification.CreateEmailVerificationTemplate(verificationLink);
            var response = await _fluentEmail
                .To(model.email)
                .Subject("Verify Your Email")
                .Body(emailBody, true)
                .SendAsync();
            if (response.Successful)
            {
                return new Result()
                {
                    Success = true,
                    Message = "Email verification link sent to your email"
                };
            }

            return new Result()
            {
                Success = false,
                Message = "Email verification link could not be sent to your email"
            };
        }
        catch (Exception ex)
        {
            return new Result()
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
    public async Task<Result> VerifyEmailAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return new Result()
            {
                Success = false,
                Message = "Invalid or expired verification link.",
                Data = null
            };

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Result()
            {
                Success = false,
                Message = "User Not Found",
                Data = null
            };

        if (user.EmailConfirmed)
            return new Result()
            {
                Success = true,
                Message = "Email already verified",
                Data = null
            };

        var result = await _userManager.ConfirmEmailAsync(user, token);
        
        if (result.Succeeded)
        {
            return new Result()
            {
                Success = true,
                Message = "Email verified successfully",
                Data = null
            };
        }

        return new Result()
        {
            Success = false,
            Message = "Email verification failed",
            Data = result.Errors
        };
    }
}