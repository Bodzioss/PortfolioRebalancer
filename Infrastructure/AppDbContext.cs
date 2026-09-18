using Microsoft.EntityFrameworkCore;
using PortfolioRebalancer.Domain;

namespace PortfolioRebalancer.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Asset> Assets { get; set; }
}
