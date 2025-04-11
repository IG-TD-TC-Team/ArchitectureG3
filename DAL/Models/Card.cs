using System;
using DAL.Models;

namespace DAL.Models
{
    public class Card
    {
        public Card(Guid userid)
        {
            CardID = Guid.NewGuid();
            UserID = userid;

            CreationDate = DateTime.UtcNow;
            ExpirationDate = CreationDate.AddYears(4);

            IsActive = true;
        }

        public Guid CardID { get; set; }

        public Guid UserID { get; set; }
        public User User { get; set; }

        // Dates
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public bool IsActive { get; set; }
    }
}