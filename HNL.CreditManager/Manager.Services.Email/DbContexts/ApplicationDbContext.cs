using Manager.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Manager.Services.Email.DbContexts
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<EmailLog> EmailLogs { get; set; }


    }
}
