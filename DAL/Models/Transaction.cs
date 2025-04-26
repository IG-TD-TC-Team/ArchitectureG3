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


        /**
        * CALCULATION HAVE TO ME MOVED TO THE BUSINESS LAYER
        */

            // Set totals based on page count
            TotalCopyQuota = pageCount;

            // Set totals based on page count and product properties
            TotalCopyQuota = (int)(pageCount * product.PrintQuotaCost); 

            // For monetary calculations
            TotalCHF = pageCount * product.PricePerUnit;

            // Set total quota in CHF           
            TotalQuotaCHF = TotalCHF;
        /**
        * CALCULATION HAVE TO ME MOVED TO THE BUSINESS LAYER
        */
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

        /// <summary>
        /// Total printing quota used in this transaction (equals page count for B/W).
        /// </summary>
        public int TotalCopyQuota { get; set; }

        /// <summary>
        /// Total monetary amount (CHF) for this transaction.
        /// </summary>
        public decimal TotalCHF { get; set; }

        /// <summary>
        /// Total quota value in CHF for this transaction.
        /// </summary>
        public decimal TotalQuotaCHF { get; set; }

        /// <summary>
        /// Applies the transaction effects to the linked user's balance.
        /// </summary>
        public void ApplyToUserBalance()
        {
            if (User != null)
            {
                // Deduct costs from user's account
                User.CopyQuota -= TotalCopyQuota;
                User.CHF -= TotalCHF;
                User.QuotaCHF -= TotalQuotaCHF;
            }
        }
    }
}