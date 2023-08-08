using Microsoft.EntityFrameworkCore;
using NessServer.Models;

namespace NessServer.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
    }
}
