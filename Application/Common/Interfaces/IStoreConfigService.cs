using Application.DTOs;
using Application.DTOs.store;

namespace Application.Common.Interfaces;

public interface IStoreConfigService
{
    Task<Result> SetPriceMarginAsync(SetPriceMarginDto dto);
}