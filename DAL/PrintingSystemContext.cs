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
        public DbSet<Card> Cards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {

            builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PrintingSystemDB")
                .UseSeeding((context, _) =>
                {
                    // Default product
                    if (!context.Products.Any(p => p.Name == "A4_BW"))
                    {
                        context.Products.Add(new Product());
                        context.SaveChanges();
                    }




                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    // Users
                    if (!await context.Users.AnyAsync(u => u.FirstName == "Tatiana", cancellationToken))
                    {
                        context.Users.Add(new User("Tatiana", "Da Costa", "AdminHevs01", context));
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    if (!await context.Users.AnyAsync(u => u.FirstName == "Julio", cancellationToken))
                    {
                        context.Users.Add(new User("Julio", "Cortès", "AdminHevs01", context));
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    if (!await context.Users.AnyAsync(u => u.FirstName == "Sofia", cancellationToken))
                    {
                        context.Users.Add(new User("Sofia", "Cortès", "AdminHevs01", context));
                        await context.SaveChangesAsync(cancellationToken);
                    }

                });


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Card)
                .WithOne(c => c.User)
                .HasForeignKey<Card>(c => c.UserID);

        }
    }
}