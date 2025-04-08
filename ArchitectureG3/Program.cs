using System;
using DAL;

namespace PrintingSystemInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Printing System Database...");

            // Create and initialize the database
            using (var context = new PrintingSystemContext())
            {
                // Create the database if it doesn't exist and seed initial data.
                context.Database.EnsureCreated();

                Console.WriteLine("Database created and seeded successfully!");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
