namespace PlasmaDonorAPI.Dto
{
    public class ProfileStatsDto
    {
        public int Count { get; set; }
        public string GroupingValue { get; set; }  // This could be state_code, city, or md_title
    }

}
