namespace NewPlasmaDonorsAPI.Dto
{
    public class DonorInfluencerDto
    {
        public long DonorId { get; set; }
        public long? InfId { get; set; }
        public string DonorEmail { get; set; }
        public string InfluencerEmail { get; set; }
        public string DonorFirstName { get; set; }
        public string DonorLastName { get; set; }
    }
}
