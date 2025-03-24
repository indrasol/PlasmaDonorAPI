using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Models
{
    public class ProfileModel
    {
        internal readonly int homeCenterId;

        [Key]
        public long id { get; set; }

        public int? age { get; set; }

        public DateTime? createdOn { get; set; }

        public DateTime? dob { get; set; }

        [StringLength(255)]
        public string? email { get; set; }

        [Required]
        [StringLength(255)]
        public string? firstName { get; set; }

        [StringLength(255)]
        public string? gender { get; set; }

        public bool? isDonor { get; set; }

        public bool? isInfluencer { get; set; }

        [StringLength(255)]
        public string? lastName { get; set; }

        [StringLength(255)]
        public string? phoneNumber { get; set; }

        [StringLength(255)]
        public string? schoolAttended { get; set; }

        public DateTime? updatedOn { get; set; }

        public long? address { get; set; }

        public long? createdBy { get; set; }

        public long? education { get; set; }

        public long? language { get; set; }

        public long? occupation { get; set; }

        public long? race { get; set; }

        public long? relationship{ get; set; }

        public long? relshipReason { get; set; }

        public long? updatedBy { get; set; }

        public long? HomeCenter { get; set; }

        [StringLength(255)]
        public string? InfluencedBy { get; set; }

        public bool? IsRelationshipActive { get; set; }

        public long? RelshipScoreId { get; set; }

        [StringLength(255)]
        public string? RelshipStatus { get; set; }

        public long? HomeCenterId { get; set; }

        public long? hobbies { get; set; }

        public string interests { get; set; }

        [StringLength(255)]
        public string? Status { get; set; }
        
        public int infScore { get; set; }

        // Navigation Properties (for foreign keys)
        public virtual Address? Address { get; set; }
        public virtual UserModel? CreatedByUser { get; set; }
        public virtual UserModel? UpdatedByUser { get; set; }
        public virtual MasterData? Education { get; set; }
        public virtual MasterData? Language { get; set; }
        public virtual MasterData? Occupation { get; set; }
        public virtual MasterData? Race { get; set; }
        public virtual MasterData? Relationship { get; set; }
        public virtual MasterData? RelshipReason { get; set; }
        public virtual MasterData? RelshipScore { get; set; }
        public virtual CompanyLocation? HomeCenterLocation { get; set; }
        
        //public virtual ProfileMdMap? ProfileMdMaps { get; set; }
    }
}
