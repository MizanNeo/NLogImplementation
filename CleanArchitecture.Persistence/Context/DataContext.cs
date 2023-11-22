using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Log> Logs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure your entity relationships, constraints, etc.
    }
}
