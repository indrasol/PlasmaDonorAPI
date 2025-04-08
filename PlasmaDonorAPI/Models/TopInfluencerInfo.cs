namespace NewPlasmaDonorsAPI.Models
{
    public class TopInfluencerInfo
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string FirstName { get; set; }      // Full name of the influencer
        public string LastName { get; set; }
        public string Email { get; set; }     // Email of the influencer
        //public string? Location { get; set; }  // Formatted location (City, State, Country)
        public double InfScore { get; set; }     // Influence Score

      
    }
}
