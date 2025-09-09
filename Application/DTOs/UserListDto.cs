namespace Application.DTOs;

public record UserListDto(
    int Id,
    string UserName,
    string Email,
    bool IsEmailVerified,
    DateTime CreatedAt,
    List<string> roles,
    string imageUrl);