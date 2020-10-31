using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpenBound_Network_Object_Library.Models
{
        public enum TransactionStatus
        {
            Pending,
            Delivered,
            Refunded,
            Cancelled
        }
        public class DonationTransaction
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
            [Required]
            public int Player_ID { get; set; }
            [ForeignKey("Player_ID")]
            public virtual Player Player { get; set; }
            public string PaymentMethod { get; set; }
            public string GatewayTransactionId { get; set; }
            [Required, Range(1, int.MaxValue)]
            public int CashAmount { get; set; }
            [Required, Range(1, int.MaxValue)]
            public int Price { get; set; }
            [Required]
            public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Pending;
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime UpdatedAt { get; set; }
        }
    }

