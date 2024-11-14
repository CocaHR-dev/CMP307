using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace CMP307.Data
{

    public class ScottishGlenContextFactory : IDesignTimeDbContextFactory<ScottishGlenContext>
    {
        public ScottishGlenContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScottishGlenContext>();

            try
            {
                // Build configuration
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Properties/appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("ScottishGlenDb");

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"An error occurred while creating the DbContext: {ex.Message}");
                throw;
            }

            return new ScottishGlenContext(optionsBuilder.Options);
        }
    }
}