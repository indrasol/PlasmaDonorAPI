using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Models
{
    public class ProfileMdMap
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long id { get; set; }

        [ForeignKey("Profile")]
        [Column("profile_id")]
        public long? profileId { get; set; }
        public virtual ProfileModel? profile { get; set; }

        [ForeignKey("MasterData")]
        [Column("md_id")]
        public long? mdId { get; set; }
        public virtual MasterData? md { get; set; }
    }
}
