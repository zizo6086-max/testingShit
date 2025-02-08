using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : GenericRepository<AppUser>(context),IUserRepository;