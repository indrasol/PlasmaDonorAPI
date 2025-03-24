using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Models
{
    public class Company
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Column("name")]
        [MaxLength(255)]
        public string? name { get; set; }

        [Column("status")]
        [MaxLength(255)]
        public string? status { get; set; }

        [NotMapped]
        public object? companyLocation { get; set; }
        
    }
}
