using Application.DTOs;
using Application.DTOs.store;

namespace Application.Common.Interfaces;

public interface IApplicationsService
{
    public Task<Result> SubmitApplicationAsync(int userId, SellerApplicationDto dto);
}