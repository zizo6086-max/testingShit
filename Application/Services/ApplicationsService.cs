using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.store;
using Application.Mappers;
using Domain.Constants;
using Domain.Models.Store;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ApplicationsService(ILogger<ApplicationsService> logger, AppDbContext context, IUnitOfWork unitOfWork, IPaginationService<SellerApplication> paginationService)
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

    public async Task<SellerApplicationListResponseDto> SearchSellerApplicationsAsync(
        int page = 1,
        int limit = 25,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 1;
        if (limit > 100) limit = 100;

        var query = context.SellerApplications
            .Include(a => a.User)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        var paginatedResult = await paginationService.GetPaginatedAsync(
            query,
            page,
            limit,
            orderBy: q => q.OrderByDescending(a => a.DateSubmitted),
            cancellationToken: cancellationToken);

        return new SellerApplicationListResponseDto
        {
            ItemCount = paginatedResult.ItemCount,
            Results = paginatedResult.Results.Select(a => a.ToListItemDto()).ToList()
        };
    }
}