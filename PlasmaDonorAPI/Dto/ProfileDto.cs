using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Dto
{
    public class ProfileDto
    {
        public long id { get; set; }

        [Required]
        public string? firstName { get; set; }

        public string? lastName { get; set; }
        public string? name { get; set; }

        public DateTime? dob { get; set; }

        public int age { get; set; }

        public string? gender { get; set; }

        public string? phoneNumber { get; set; }

        [Required]
        public string? email { get; set; }

        public bool? isDonor { get; set; }

        public bool? isInfluencer { get; set; }

        public List<long>? influencerIds { get; set; }

        public string? influencers { get; set; }

        public string? education { get; set; }

        public long? educationId { get; set; }

        public string? realtionshipScore { get; set; }

        public long? realtionshipScoreId { get; set; }

        public double infScore { get; set; }

        public string? fullAddress { get; set; }

        public string? addressLine1 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? stateCode { get; set; }
        public string? country { get; set; }
        public string? countryCode { get; set; }
        public string? postalCode { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? schoolAttended { get; set; }

        public List<string> interests = new List<string>();
        public string? interestStr { get; set; }
        public List<long>? interestIds { get; set; }

        public List<string>? hobbies = new List<string>();
        public string? hobbieStr { get; set; }
        public List<long>? hobbiesIds { get; set; }
        public long homeCenterId { get; set; }
        public string? homeCenter { get; set; }
        public string? createdOn { get; set; }
        public int ageGroup { get; set; }
        public string? relshipStatus { get; set; }
        public string? occupation { get; set; }

        public long? occupationId { get; set; }

        public string? relationship { get; set; }

        public long? relationshipId { get; set; }

        public string? race { get; set; }

        public long? raceId { get; set; }

        public string? address { get; set; }

        public long? addressId { get; set; }

        public string? language { get; set; }

        public long? languageId { get; set; }

        public string? status { get; set; }
        public String? influencedBy { get; set; }

        public long influencedById { get; set; }

        public List<ProfileDto> children = new List<ProfileDto>();
    }
}
