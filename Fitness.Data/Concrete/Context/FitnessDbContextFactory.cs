using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Fitness.Data.Concrete.Context
{
    public class FitnessDbContextFactory : IDesignTimeDbContextFactory<FitnessDbContext>
    {
        public FitnessDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()

                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Fitness"))


                .AddJsonFile("appsettings.json", optional: true)


                .AddJsonFile("appsettings.Development.json", optional: true)

                .Build();


            var connectionString = configuration.GetConnectionString("DefaultConnection");


            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection String 'DefaultConnection' has not been found or initialized in appsettings.Development.json.");
            }


            var builder = new DbContextOptionsBuilder<FitnessDbContext>();
            builder.UseSqlServer(connectionString);

            return new FitnessDbContext(builder.Options);
        }
    }
}