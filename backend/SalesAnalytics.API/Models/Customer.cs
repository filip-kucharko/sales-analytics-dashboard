using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.API.Models
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // One Customer has Many Sales
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}