// PrintingSystemContext.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    /// <summary>
    /// Entity Framework database context for the printing system.
    /// Defines the tables and relationships used in the database.
    /// </summary>
    public class PrintingSystemContext : DbContext
    {
        // DbSets represent tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Card> Cards { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        /// <summary>
        /// Configures the database connection if it hasn't been configured externally.
        /// </summary>
        /// <param name="optionsBuilder">Used to build options for the DbContext.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure if not already configured (e.g. via dependency injection)
            if (!optionsBuilder.IsConfigured)
            {
                // Set SQL Server as the database provider with a local database
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=PrintingSystemDB;Trusted_Connection=True");
            }
        }

        /// <summary>
        /// Configures the model by defining table names, relationships, and field properties.
        /// </summary>
        /// <param name="modelBuilder">Used to define the database schema.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set custom table names to match plural forms
            // Configure the Product table
            modelBuilder.Entity<Product>().ToTable("Products");
            // Configure the User table
            modelBuilder.Entity<User>().ToTable("Users");
            // Configure the Card table
            modelBuilder.Entity<Card>().ToTable("Cards");
            // Configure Transaction table
            modelBuilder.Entity<Transaction>().ToTable("Transactions");
            // Configure TransactionProducts table
            modelBuilder.Entity<TransactionProduct>().ToTable("TransactionProducts");

            // Define one-to-one relationship between User and Card
            modelBuilder.Entity<User>()
                .HasOne(u => u.Card)              // A User has one Card
                .WithOne(c => c.User)             // A Card belongs to one User
                .HasForeignKey<Card>(c => c.UserID) // The foreign key is on the Card entity
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, delete the card too

            // Define relationship between User and Transactions (one-to-many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, delete their transactions too

            // Define relationship between Transaction and TransactionProducts (one-to-many)
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.TransactionProducts)
                .WithOne(tp => tp.Transaction)
                .HasForeignKey(tp => tp.TransactionID)
                .OnDelete(DeleteBehavior.Cascade); // If the transaction is deleted, delete its products too

            // Define relationship between TransactionProducts and Product (many-to-one)
            modelBuilder.Entity<TransactionProduct>()
                .HasOne(tp => tp.Product)
                .WithMany()  // Product doesn't need to track which TransactionProducts entries it's in
                .HasForeignKey(tp => tp.ProductID)
                .OnDelete(DeleteBehavior.Restrict);  // Don't delete products when a transaction is deleted

            // Set precision for PricePerUnit field in Product (important for money values)
            modelBuilder.Entity<Product>()
                .Property(p => p.PricePerUnit)
                .HasColumnType("decimal(18,2)"); // 18 total digits, 2 after decimal


        }
    }
}
