using System;
using DAL;

namespace PrintingSystemInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Julio the best
            Console.WriteLine("Initializing Printing System Database...");

            using (var context = new PrintingSystemContext())
            {
                // Create the database and schema
                context.Database.EnsureDeleted(); // Optional: To start from 0 for testing
                context.Database.EnsureCreated();

                // Seed the Data
                try
                {
                    DatabaseInitializator.SeedData(context);
                    Console.WriteLine("Database initialized successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Initialization failed: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}