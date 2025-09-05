using Application.Common.Interfaces;
using Application.DTOs;
using Application.DTOs.store;
using Domain.Models.Store;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class StoreConfigService(AppDbContext context) : IStoreConfigService
{
    private readonly AppDbContext _context = context;

    public async Task<Result> SetPriceMarginAsync(SetPriceMarginDto dto)
    {
        var margin = dto.Margin;
        if (margin is <= 0 or >= 1)
        {
            return new Result()
            {
                Message = "Margin must be greater than Zero and less than 1"
            };
        }
        var existingConfig = await _context.StoreConfigs.FirstOrDefaultAsync();
        if (existingConfig != null)
            existingConfig.PriceMargin = margin;
        else
            await _context.StoreConfigs.AddAsync(existingConfig = new StoreConfigs
            { PriceMargin = margin });
        
        if (await _context.SaveChangesAsync() > 0 || existingConfig.PriceMargin.Equals(margin))
        {
            return new Result()
            {
                Success = true,
                Message = "Margin Updated"
            };
        }
        return new Result()
        {
            Success = false,
            Message = "Margin could not be updated"
        };

    }
    
}