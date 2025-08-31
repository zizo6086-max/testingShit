using Application.DTOs;

namespace Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result> SendEmailVerificationAsync(EmailVerificationRequest model);
    Task<Result> VerifyEmailAsync(string userId, string token);
}
