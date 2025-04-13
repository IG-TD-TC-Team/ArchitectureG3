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
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Card>().ToTable("Cards");

            // Define one-to-one relationship between User and Card
            modelBuilder.Entity<User>()
                .HasOne(u => u.Card)              // A User has one Card
                .WithOne(c => c.User)             // A Card belongs to one User
                .HasForeignKey<Card>(c => c.UserID) // The foreign key is on the Card entity
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, delete the card too

            // Set precision for PricePerUnit field in Product (important for money values)
            modelBuilder.Entity<Product>()
                .Property(p => p.PricePerUnit)
                .HasColumnType("decimal(18,2)"); // 18 total digits, 2 after decimal
        }
    }
}
