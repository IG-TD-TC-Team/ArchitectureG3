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
        /// Seeds sample printing transactions into the database.
        /// </summary>
        private static void SeedTransactions(PrintingSystemContext context)
        {
            // Check if we already have transactions in the database
            if (context.Transactions.Any())
            {
                return; // Skip if transactions already exist
            }

            // Get all users and products for creating sample transactions
            var users = context.Users.ToList();
            var products = context.Products.ToList();

            // Exit if we don't have both users and products
            if (!users.Any() || !products.Any())
            {
                Console.WriteLine("Cannot seed transactions: Users or products missing");
                return;
            }

            var random = new Random(42); // Using a seed for reproducible results

            // For each user, create 1-3 transactions
            foreach (var user in users)
            {
                int transactionCount = random.Next(1, 4); // 1 to 3 transactions per user

                for (int i = 0; i < transactionCount; i++)
                {
                    // Create a new transaction for this user
                    var transaction = new Transaction(user.UserID)
                    {
                        // Set the date to sometime in the last 30 days
                        Date = DateTime.UtcNow.AddDays(-random.Next(0, 30))
                    };

                    // Determine how many different products in this transaction (1-3)
                    int productCount = random.Next(1, 4);

                    // Track transaction totals
                    int totalCopyQuota = 0;
                    decimal totalCHF = 0m;
                    decimal totalQuotaCHF = 0m;

                    // Add 1-3 random products to this transaction
                    for (int j = 0; j < productCount; j++)
                    {
                        // Select a random product
                        var product = products[random.Next(products.Count)];

                        // Determine quantity (1-20 copies)
                        int quantity = random.Next(1, 21);

                        // Calculate costs for this transaction product
                        int copyQuotaCost = (int)(product.PrintQuotaCost * quantity);
                        decimal chfCost = product.PricePerUnit * quantity;
                        decimal quotaCHFCost = chfCost; // In a real system, these might differ

                        // Create the transaction product relationship
                        var transactionProduct = new TransactionProduct(
                            transaction.TransactionID,
                            product.ProductID,
                            quantity)
                        {
                            CopyQuotaCost = copyQuotaCost,
                            CHFCost = chfCost,
                            QuotaCHFCost = quotaCHFCost
                        };

                        // Add to the transaction
                        transaction.TransactionProducts.Add(transactionProduct);

                        // Update transaction totals
                        totalCopyQuota += copyQuotaCost;
                        totalCHF += chfCost;
                        totalQuotaCHF += quotaCHFCost;
                    }

                    // Set the transaction totals
                    transaction.TotalCopyQuota = totalCopyQuota;
                    transaction.TotalCHF = totalCHF;
                    transaction.TotalQuotaCHF = totalQuotaCHF;

                    // Add the transaction to the context
                    context.Transactions.Add(transaction);

                    // Update the user's balance - this simulates the effect of the transaction
                    // Note: this would be handled by business logic
                    user.CopyQuota -= totalCopyQuota;
                    user.CHF -= totalCHF;
                    user.QuotaCHF -= totalQuotaCHF;
                }
            }

            // Save all transaction changes to the database
            context.SaveChanges();
            Console.WriteLine($"Added {context.Transactions.Count()} sample transactions");
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
