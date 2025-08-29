namespace Domain.Models.Auth;

public static class UserRoles
{
    public const string User = "User";
    public const string Admin = "Admin";
    public const string Seller = "Seller";
    public static readonly string[] All = [User,Admin,Seller];
}