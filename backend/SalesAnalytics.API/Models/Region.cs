using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.API.Models
{
    [Table("Regions")]
    public class Region
    {
        [Key]
        public int RegionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // One Region has Many Sales
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}