using Microsoft.Identity.Client;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.Dto
{
    public class DashboardStatsDto
    {
        public int DonorCount { get; set; }
        public int InfluencerCount { get; set; }
        public int RecentInfluencerCount { get; set; }
        public int RecentDonorCount { get; set; }
		public List<Tuple<string, int>>? DonorTimeSeries { get; set; }
		public List<Tuple<string, int>>? InfluencerTimeSeries { get; set; }

		public List<Tuple<string, int>>? ProfileByState { get; set; }
		public List<Tuple<string, int>>? DonorByOccupation { get; set; }

        public List<TopInfluencerInfo>? TopInfluencers { get; set; }
    }
}
