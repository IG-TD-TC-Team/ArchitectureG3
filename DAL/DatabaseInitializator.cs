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
        /// Seeds the database with initial products and users.
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
        /// Seeds predefined printing products into the database if they don't already exist.
        /// </summary>
        private static void SeedProducts(PrintingSystemContext context)
        {
            // Check if the basic A4 Black and White product exists
            if (!context.Products.Any(p => p.Name == "A4_BW"))
            {
                // If not, add a default product (likely a placeholder)
                context.Products.Add(new Product());
                context.SaveChanges();
            }

            // Define additional product configurations
            var additionalProducts = new[]
            {
                new { Name = "A4_Color", Description = "A4 Color Print", Price = 0.25M, Color = true, Size = "A4", Type = "StandardPaper" },
                new { Name = "A3_BW", Description = "A3 Black and White", Price = 0.2M, Color = false, Size = "A3", Type = "StandardPaper" },
                new { Name = "A3_Color", Description = "A3 Color Print", Price = 0.5M, Color = true, Size = "A3", Type = "StandardPaper" }
            };

            // Add each additional product only if it doesn't already exist
            foreach (var prod in additionalProducts)
            {
                if (!context.Products.Any(p => p.Name == prod.Name))
                {
                    // Create a new Product using a constructor that automatically sets derived fields
                    var product = new Product(
                        prod.Name,
                        prod.Description,
                        prod.Price,
                        prod.Color,
                        prod.Size,  // Maps to PaperSize enum or property
                        prod.Type   // Maps to PaperType enum or property
                    );

                    context.Products.Add(product);
                }
            }

            context.SaveChanges(); // Persist all changes
        }

        /// <summary>
        /// Seeds a list of default users into the system with generated usernames.
        /// </summary>
        private static void SeedUsers(PrintingSystemContext context)
        {
            // List of user info to add
            var usersToSeed = new[]
            {
                new { FirstName = "Tatiana", LastName = "Da Costa", Password = "AdminHevs01" },
                new { FirstName = "Julio", LastName = "Cortès", Password = "AdminHevs01" },
                new { FirstName = "Sofia", LastName = "Cortès", Password = "AdminHevs01" }
            };

            // Check if user already exists, and add if not
            foreach (var user in usersToSeed)
            {
                if (!context.Users.Any(u => u.FirstName == user.FirstName && u.LastName == user.LastName))
                {
                    // Generate a unique username to avoid duplicates
                    var username = GenerateUniqueUsername(context, user.FirstName, user.LastName);

                    // Create and add the new user
                    context.Users.Add(new User(
                        user.FirstName,
                        user.LastName,
                        user.Password,
                        username
                    ));

                    context.SaveChanges(); // Save after each user
                }
            }
        }

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
