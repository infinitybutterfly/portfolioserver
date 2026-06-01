using Microsoft.EntityFrameworkCore;
using Portfoilio_Server.Models;

namespace Portfoilio_Server.Data
{
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
    }
}
