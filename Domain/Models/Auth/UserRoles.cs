using Domain.Constants;

namespace Domain.Models.Auth;

public static class UserRoles
{
    public const string User = AuthConstants.Roles.User;
    public const string Admin = AuthConstants.Roles.Admin;
    public const string Seller = AuthConstants.Roles.Seller;
    public static readonly string[] All = [User, Admin, Seller];
}