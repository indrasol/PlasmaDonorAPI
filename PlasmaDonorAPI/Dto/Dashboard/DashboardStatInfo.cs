using NewPlasmaDonorsAPI.Models;
using PlasmaDonorAPI.Dto;

namespace NewPlasmaDonorsAPI.Dto.Dashboard
{
    [Serializable]
    public class DashboardStatInfo
    {

        public List<KpiInfo> topCards { get; set; } = new List<KpiInfo>();
        public List<KpiInfo> infSeries { get; set; } = new List<KpiInfo>();
        public List<KpiInfo> donorSeries { get; set; } = new List<KpiInfo>();
        public List<KpiInfo> pfsByStates { get; set; } = new List<KpiInfo>();
        public List<KpiInfo> pfsByOccupation { get; set; } = new List<KpiInfo>();
        public List<TopInfluencerDto> topInfluencers { get; set; } = new List<TopInfluencerDto>();
        
        public List<KpiInfo> pfsByCities { get; set; } = new List<KpiInfo>();      // List of KPIs for profiles by cities

        public List<KpiInfo> pfsByRels { get; set; } = new List<KpiInfo>();           // List of KPIs for profiles by relationships
        public List<KpiInfo> pfsByEdu { get; set; } = new List<KpiInfo>();           // List of KPIs for profiles by education
    }
}
