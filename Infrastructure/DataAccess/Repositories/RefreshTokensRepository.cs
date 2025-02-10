using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class RefreshTokensRepository(AppDbContext context) : GenericRepository<RefreshToken>(context), IRefreshTokenRepository;