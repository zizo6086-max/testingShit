using Application.DTOs;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class AuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    private async Task<Result> CheckExistence(RegisterDto registerDto)
    {
        var userExists = await _userManager.FindByNameAsync(registerDto.Username);
        var result = new Result();
        if (userExists != null)
        {
            result.Message = "Username already exists!";
            return result;
        }
        userExists = await _userManager.FindByEmailAsync(registerDto.Email);
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
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var creationResult = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!creationResult.Succeeded)
            {
                transaction.Rollback();
                result.Success = false;
                result.Message = "Failed to create new user!";
            }

            if (!(await _userManager.AddToRoleAsync(newUser, role)).Succeeded)
            {
                transaction.Rollback();
                result.Success = false;
                result.Message = "Failed to assign role to new user!";
                return result;
            }

            await _unitOfWork.CommitAsync();
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