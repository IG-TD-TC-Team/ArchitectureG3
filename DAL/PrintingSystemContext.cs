using System;
using System.Linq;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class PrintingSystemContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Balance> Balances { get; set; }

        // Seed data arrays. Since AbstractUser is abstract, make sure you use concrete types
        private static readonly User[] _users = new User[]
        {
            new User("Julio", "Julio", "Cortés", "Test"),
            new User("Pablo", "Pablo", "Escobar", "Test2025")
        };

        private static readonly Product[] _products = new Product[]
        {
            new Product("Common printing", 10)
        };

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // Use your SQL Server connection string
            builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PrintingSystemDB")
                // Assuming UseSeeding is a valid extension in your project;
                // if not, consider using modelBuilder.HasData in OnModelCreating.
                .UseSeeding((context, _) =>
                {
                    // Seed users
                    foreach (var user in _users)
                    {
                        // Note: using a concrete type is necessary because AbstractUser is abstract.
                        var existingUser = context.Set<User>()
                            .FirstOrDefault(u => u.UserID == user.UserID);

                        if (existingUser == null)
                        {
                            context.Set<User>().Add(user);
                        }
                    }
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("❌ SaveChanges failed (Users): " + ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("🔍 Inner Exception: " + ex.InnerException.Message);
                    }

                    // Seed products
                    foreach (var product in _products)
                    {
                        var existingProduct = context.Set<Product>()
                            .FirstOrDefault(p => p.ProductId == product.ProductId);

                        if (existingProduct == null)
                        {
                            context.Set<Product>().Add(product);
                        }
                    }
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("❌ SaveChanges failed (Products): " + ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("🔍 Inner Exception: " + ex.InnerException.Message);
                    }

                    // Create initial balances for users if needed
                    foreach (var user in context.Set<User>().ToList())
                    {
                        if (!user.Balances.Any())
                        {
                            var balance = new Balance
                            {
                                User = user,
                                MoneyCHF = 0,
                                StandardQuotaCHF = 100,
                                TransactionDate = DateTime.Now
                                
                            };
                            context.Set<Balance>().Add(balance);
                            user.Balances.Add(balance);
                        }
                    }
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SaveChanges failed (Balances): " + ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                    }
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure AbstractUser as the base type using a discriminator column.
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User");
            // .HasValue<Professor>("Professor"); // Uncomment if adding more user types.

            // Configure AbstractProduct as the base type using a discriminator column.
            modelBuilder.Entity<Product>()
                .HasDiscriminator<string>("ProductType")
                .HasValue<Product>("Product");
            // .HasValue<A4Color>("A4Color");  // Uncomment if needed.

            // Configure primary keys.
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);

            // Configure composite key for Balance.
            // Consider switching to BalanceId as PK if TransactionDate might collide.
            // Use BalanceId as the primary key.
            modelBuilder.Entity<Balance>().HasKey(b => b.BalanceId);


            // Configure relationship: One User has many Balances.
            modelBuilder.Entity<User>()
                .HasMany(u => u.Balances)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserID);

            // Configure relationship: One Balance has many Products.
            modelBuilder.Entity<Balance>()
                .HasMany(b => b.Products)
                .WithOne(p => p.Balance)
                .HasForeignKey(p => p.BalanceId)
                .IsRequired(false); 
        }
    }
}
