using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectbombo.Models
{
    public class Debt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LenderId { get; set; } // Borcu veren kişi

        [Required]
        public int BorrowerId { get; set; } // Borcu alan kişi

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Borç miktarı

        [Required]
        public DateTime Date { get; set; } = DateTime.Now; // Borç tarihi

        public bool IsPaid { get; set; } = false; // Ödeme durumu

        [ForeignKey("LenderId")]
        public User Lender { get; set; }

        [ForeignKey("BorrowerId")]
        public User Borrower { get; set; }
    }
}
