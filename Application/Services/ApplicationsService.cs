using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.store;
using Application.Mappers;
using Domain.Constants;
using Domain.Models.Store;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ApplicationsService(ILogger<ApplicationsService> logger, AppDbContext context, IUnitOfWork unitOfWork)
    : IApplicationsService
{

    public async Task<Result> SubmitApplicationAsync(int userId, SellerApplicationDto dto)
    {
        var user = context.Users.FirstOrDefault(x => x.Id == userId);
        if (user == null)
        {
            return new Result()
            {
                Message = "User not found"
            };
        }
        var exitingApplication = context.SellerApplications
            .FirstOrDefault(x => x.UserId == userId);
        if (exitingApplication is 
            { Status: SellerApplicationConstants.Approved or SellerApplicationConstants.Pending }) 
        {
            return new Result()
            {
                Message = "You have already submitted an application and is approved or pending"
            };
        }

        var application = dto.ToEntity();
        context.SellerApplications.Add(application);
        if (await context.SaveChangesAsync() > 0)
        {
            return new Result()
            {
                Success = true,
                Message = "Application submitted successfully"
            };
        }
        return new Result()
        {
            Message = "Something went wrong contact support"
        };
    }


}