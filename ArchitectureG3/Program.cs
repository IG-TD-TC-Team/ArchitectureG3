using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PrintingSystemInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Azure Printing System Database...");

            try
            {
                // Build configuration from appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("Connection string not found in appsettings.json");
                    return;
                }

                Console.WriteLine("Connecting to Azure SQL Database...");
                var optionsBuilder = new DbContextOptionsBuilder<PrintingSystemContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var context = new PrintingSystemContext(optionsBuilder.Options))
                {
                    // Test connection
                    Console.WriteLine("Testing database connection...");
                    var canConnect = context.Database.CanConnect();
                    if (!canConnect)
                    {
                        Console.WriteLine("Cannot connect to database. Check connection string and firewall rules.");
                        return;
                    }

                    Console.WriteLine("Connection successful!");

                    // Create the database and schema
                    Console.WriteLine("Creating database schema...");
                    context.Database.EnsureCreated();

                    // Seed the Data
                    Console.WriteLine("Seeding initial data...");
                    DatabaseInitializator.SeedData(context);
                    Console.WriteLine("Database initialized successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}