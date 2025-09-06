namespace Application.DTOs.store;

public record SellerApplicationDto(
    int userId,
    DateTime DateOfBirth,
    string PhoneNumber,
    string IdCardUrl,
    string City,
    string Country,
    string PostalCode,
    string Address,
    string Note, 
    DateTime DateSubmitted);