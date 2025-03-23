using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EasyTeams.Data.Models.Domain;

namespace EasyTeams.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                string? conn = config.GetConnectionString("EasyTeamsContext");
                builder.UseSqlServer(conn);
                base.OnConfiguring(builder);
            }
        }
       // public DbSet<EasyTeams.Data.Models.Domain.PTask> PTask { get; set; } = default!;
       

    }
}
