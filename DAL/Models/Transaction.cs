using System;

namespace DAL.Models
{
    /// <summary>
    /// Represents a black and white A4 printing transaction in the system.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Private constructor for Entity Framework.
        /// </summary>
        private Transaction() { }

        /// <summary>
        /// Creates a new A4 B/W printing transaction for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user making the transaction.</param>
        /// <param name="pageCount">Number of A4 B/W pages printed.</param>
        public Transaction(Guid userId, int pageCount, Product product)
        {

            TransactionID = Guid.NewGuid();
            Date = DateTime.UtcNow;
            UserID = userId;
            PageCount = pageCount;
            Product = product;
            ProductID = product.ProductID;  // Set the foreign key

        }

        /// <summary>
        /// Unique identifier for the transaction.
        /// </summary>
        public Guid TransactionID { get; set; }

        /// <summary>
        /// Date and time when the transaction occurred.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Foreign key to the user who made the transaction.
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Navigation property to access the associated User.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Number of A4 B/W pages printed in this transaction.
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Foreign key to the product used in this transaction.
        /// </summary>
        public Guid ProductID { get; set; }

        /// <summary>
        /// Product used in this transaction.
        /// </summary>
        public Product Product { get; set; }

        // ───── Transaction Totals ─────
        //Can be positive if adding quota, negative if subtracting quota


        /// <summary>
        /// Total printing quota used in this transaction (equals page count for B/W).
        /// </summary>
        public int TotalCopyQuotaInTransaction { get; set; }

        /// <summary>
        /// Total monetary amount (CHF) for this transaction.
        /// </summary>
        public decimal TotalCHFInTransaction { get; set; }

        /// <summary>
        /// Total quota value in CHF for this transaction.
        /// </summary>
        public decimal TotalQuotaCHFInTransaction { get; set; }

    }
}