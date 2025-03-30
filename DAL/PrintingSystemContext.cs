using System;
using System.Linq;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class PrintingSystemContext : DbContext
    {
        public DbSet<AbstractUser> Users { get; set; }
        public DbSet<AbstractProduct> Products { get; set; }
        public DbSet<Balance> Balances { get; set; }

        private static AbstractUser[] _users = new[]
        {
            new Student("Julio", "Julio", "Cortés", "Test"),
            new Student("Pablo", "Pablo", "Escobar", "Test2025")
        };

        private static AbstractProduct[] _products = new[]
        {
            new A4BlackAndWhite("Common printing", 10 )


        };

    }

}
