using Microsoft.EntityFrameworkCore;

namespace login.api.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Game> Games {get; set;}
    public DbSet<AppUser> AppUsers {get; set;}
}