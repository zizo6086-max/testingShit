using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppUser:IdentityUser<int>
{
    public string UserName { get; set; }
}