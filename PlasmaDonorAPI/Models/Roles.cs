using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Models
{
    public class Roles
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Column("role")]
        [MaxLength(255)]
        public string? role { get; set; }

        [Column("status")]
        [MaxLength(255)]
        public string? status { get; set; }

        [Column("title")]
        [MaxLength(255)]
        public string? title { get; set; }
    }
}
