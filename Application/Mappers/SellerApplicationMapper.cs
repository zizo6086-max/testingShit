using Application.DTOs.store;
using Domain.Models.Store;

namespace Application.Mappers;

public static class SellerApplicationMapper
{
    public static SellerApplicationDto ToDto(this SellerApplication application)
    {
        return new SellerApplicationDto(
            application.UserId,
            application.DateOfBirth,
            application.PhoneNumber,
            application.Country,
            application.City,
            application.PostalCode,
            application.IdCardUrl,
            application.Note,
            application.Note, 
            application.DateSubmitted);
    }

    public static SellerApplication ToEntity(this SellerApplicationDto dto, int userId)
    {
        return new SellerApplication()
        {
            UserId = userId,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Country = dto.Country,
            City = dto.City,
            PostalCode = dto.PostalCode,
            IdCardUrl = dto.IdCardUrl,
            Note = dto.Note,
            DateOfBirth = dto.DateOfBirth,
        };
    }

    public static SellerApplicationListItemDto ToListItemDto(this SellerApplication application)
    {
        return new SellerApplicationListItemDto
        {
            Id = application.Id,
            UserId = application.UserId,
            UserName = application.User?.UserName ?? string.Empty,
            UserEmail = application.User?.Email ?? string.Empty,
            PhoneNumber = application.PhoneNumber,
            City = application.City,
            Country = application.Country,
            IdCardUrl = application.IdCardUrl,
            Status = application.Status,
            DateSubmitted = application.DateSubmitted
        };
    }
}