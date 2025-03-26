namespace PlasmaDonorAPI.Dto
{
    [Serializable]
    public class TopInfluencerDto
    {
        public string? name {  get; set; }
        public string? email { get; set; }
        public double count { get; set; }
        public string? location { get; set; }
        public string? icn { get; set; }
        public string?  cssCls { get; set; }
    }
}
