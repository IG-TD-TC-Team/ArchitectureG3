using System;
using DAL;

namespace PrintingSystemInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Printing System Database...");

            // Create and initialize the database by applying pending migrations.
            using (var context = new PrintingSystemContext())
            {
                DbInitializer.Migrate();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
