using System;
using DAL.Models;

namespace DAL.Models
{
    /// <summary>
    /// Represents a printing system access card assigned to a user.
    /// </summary>
    public class Card
    {
        // Required by Entity Framework Core for model binding
        private Card() { }

        /// <summary>
        /// Constructor to initialize a new card for a given user.
        /// </summary>
        /// <param name="userid">The ID of the user who owns the card.</param>
        public Card(Guid userid)
        {
            CardID = Guid.NewGuid();             // Generate a unique ID for the card
            UserID = userid;                     // Link the card to a user

            CreationDate = DateTime.UtcNow;      // Set the card's creation date
            ExpirationDate = CreationDate.AddYears(4); // Set expiration to 4 years later

            IsActive = true;                     // Mark the card as active by default
        }

        /// <summary>
        /// Unique identifier for the card.
        /// </summary>
        public Guid CardID { get; set; }

        /// <summary>
        /// Foreign key linking to the User who owns this card.
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Navigation property to access the associated User.
        /// </summary>
        public User User { get; set; }

        // ----- Date fields -----

        /// <summary>
        /// The date when the card was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The date when the card will expire (4 years after creation).
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Indicates whether the card is currently active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
