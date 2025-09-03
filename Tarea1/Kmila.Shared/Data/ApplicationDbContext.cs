using Kmila.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Kmila.Shared.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}