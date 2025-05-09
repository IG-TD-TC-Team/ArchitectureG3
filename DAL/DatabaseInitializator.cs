using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;

namespace DAL
{
    /// <summary>
    /// Static class responsible for seeding initial data into the database.
    /// </summary>
    public static class DatabaseInitializator
    {
        /// <summary>
        /// Seeds the database with initial products, users, and sample transactions.
        /// </summary>
        /// <param name="context">The database context used for accessing entities.</param>
        public static void SeedData(PrintingSystemContext context)
        {
            try
            {
                // Seed initial products
                SeedProducts(context);

                // Seed initial users
                SeedUsers(context);

                // Seed sample transactions
                SeedTransactions(context);
            }
            catch (Exception ex)
            {
                // Log error details in case of failure
                Console.WriteLine($"Data seeding failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Rethrow exception to notify calling code
            }
        }

        /// <summary>
        /// Seeds only the A4 Black and White product into the database if it doesn't already exist.
        /// </summary>
        private static void SeedProducts(PrintingSystemContext context)
        {
            try
            {
                // Check if any A4_BW product already exists
                if (!context.Products.Any(p => p.Name == "A4_BW"))
                {
                    Console.WriteLine("Adding A4 Black and White product to database...");

                    // Create the default A4 B/W product
                    // The parameterless constructor creates an A4_BW product with default values
                    var a4bwProduct = new Product();

                    // Add it to the context
                    context.Products.Add(a4bwProduct);

                    // Save changes to the database
                    context.SaveChanges();

                    Console.WriteLine("A4 Black and White product added successfully.");
                }
                else
                {
                    Console.WriteLine("A4 Black and White product already exists in database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding A4 Black and White product: {ex.Message}");
                throw; // Re-throw to be caught by the main SeedData method
            }
        }

        /// <summary>
        /// Seeds a list of default users into the system with generated usernames.
        /// </summary>
        private static void SeedUsers(PrintingSystemContext context)
        {
            // List of user info to add - now including the Group property
            var usersToSeed = new[]
            {
                new { FirstName = "Tatiana", LastName = "Da Costa", Password = "AdminHevs01", Group = "staff" },
                new { FirstName = "Julio", LastName = "Cortés", Password = "AdminHevs01", Group = "staff" },
                new { FirstName = "Sofia", LastName = "Cortés", Password = "AdminHevs01", Group = "student" }
            };

            // Check if user already exists, and add if not
            foreach (var user in usersToSeed)
            {
                if (!context.Users.Any(u => u.FirstName == user.FirstName && u.LastName == user.LastName))
                {
                    // Generate a unique username to avoid duplicates
                    var username = GenerateUniqueUsername(context, user.FirstName, user.LastName);

                    // Create and add the new user - updated constructor with group parameter
                    context.Users.Add(new User(
                        user.FirstName,
                        user.LastName,
                        user.Password,
                        username,
                        user.Group  // Added the group parameter here
                    ));

                    context.SaveChanges(); // Save after each user
                }
            }
        }

        /// <summary>
        /// Seeds sample printing transactions into the database.
        /// </summary>
        private static void SeedTransactions(PrintingSystemContext context)
        {
            // Skip if transactions exist
            if (context.Transactions.Any())
                return;

            // Get users and the default A4 B/W product
            var users = context.Users.ToList();
            var product = context.Products.FirstOrDefault(p => p.Name == "A4_BW");

            if (!users.Any() || product == null)
            {
                Console.WriteLine("Cannot seed transactions: Users or product missing");
                return;
            }

            var random = new Random(42);

            foreach (var user in users)
            {
                int transactionCount = random.Next(1, 4);

                for (int i = 0; i < transactionCount; i++)
                {
                    int pageCount = random.Next(1, 51);

                    // Create transaction with product
                    var transaction = new Transaction(user.UserID, pageCount, product)
                    {
                        Date = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                        User = user  // Set user reference for applying balance changes
                    };

                    // Apply costs to user balance
                    transaction.ApplyToUserBalance();

                    context.Transactions.Add(transaction);
                }
            }

            context.SaveChanges();
            Console.WriteLine($"Added {context.Transactions.Count()} sample transactions");
        }

        /**
         * CALCULATION HAVE TO ME MOVED TO THE BUSINESS LAYER
         */
        /// <summary>
        /// Generates a unique username based on the first and last name.
        /// The base username is composed of the first 3 letters of each name.
        /// If the username already exists, a number is appended and incremented until unique.
        /// </summary>
        private static string GenerateUniqueUsername(PrintingSystemContext context, string firstName, string lastName)
        {
            // Take first 3 characters of first and last name (or less if shorter)
            string baseUsername = (firstName.Substring(0, Math.Min(firstName.Length, 3)) +
                                   lastName.Substring(0, Math.Min(lastName.Length, 3)))
                                  .ToLower();

            string username = baseUsername;
            int counter = 1;

            // Keep appending a counter until a unique username is found
            while (context.Users.Any(u => u.Username == username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }
    }
}