using Application.DTOs;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
namespace Application.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IConfiguration configuration)
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
}