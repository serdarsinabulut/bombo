using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projectbombo.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public List<Debt> DebtsGiven { get; set; } = new List<Debt>(); // Verdiği borçlar
        public List<Debt> DebtsTaken { get; set; } = new List<Debt>(); // Aldığı borçlar
    }
}
