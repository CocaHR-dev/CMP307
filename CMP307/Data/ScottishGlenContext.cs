using System.IO;
using CMP307.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CMP307.Data
{
    public partial class ScottishGlenContext : DbContext
    {
        public ScottishGlenContext() { }

        public ScottishGlenContext(DbContextOptions<ScottishGlenContext> options)
            : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Hardware> HardwareAssets => Set<Hardware>();
        public DbSet<Department> Departments => Set<Department>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Build configuration
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Properties/appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("ScottishGlenDb");

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
    }
}