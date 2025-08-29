using Domain.Models;
using Domain.Models.Auth;

namespace Infrastructure.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : GenericRepository<AppUser>(context),IUserRepository;