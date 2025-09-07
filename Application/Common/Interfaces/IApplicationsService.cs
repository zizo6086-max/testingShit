using Application.DTOs;
using Application.DTOs.store;
using System.Linq.Expressions;
using Domain.Models.Store;

namespace Application.Common.Interfaces;

public interface IApplicationsService
{
    public Task<Result> SubmitApplicationAsync(int userId, SellerApplicationDto dto);
    
    public Task<SellerApplicationListResponseDto> SearchSellerApplicationsAsync(
        int page = 1,
        int limit = 25,
        string? status = null,
        CancellationToken cancellationToken = default);
    public Task<Result> GetApplicationAsync(int userId, int id, CancellationToken cancellationToken = default); 
}