using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Models
{
    [Table("donar_influencer_map")]
    public class DonarInfluencerMap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        // Navigation property for ProfileId (many-to-one relationship)
        [ForeignKey("Profile")]
        public long profileId { get; set; }
        public virtual ProfileModel? Profile { get; set; }

        // Navigation property for InfluencedBy (many-to-one relationship)
        [ForeignKey("InfluencerProfile")]
        public long influencedBy { get; set; }
        public virtual ProfileModel? InfluencerProfile { get; set; }
    }
}
