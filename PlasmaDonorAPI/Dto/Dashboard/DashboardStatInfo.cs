namespace NewPlasmaDonorsAPI.Dto.Dashboard
{
    public class DashboardStatInfo
    {
        public List<KpiInfo> topCards;             // List of KPIs for top cards
        public List<KpiInfo> donorSeries;          // List of KPIs for donor statistics
        public List<KpiInfo> infSeries;            // List of KPIs for influencer statistics
        public List<TopInfluencersinfo> topInfluencers; // List of top influencers' information
        public List<KpiInfo> pfsByStates;         // List of KPIs for profiles by states
        public List<KpiInfo> pfsByCities;         // List of KPIs for profiles by cities
        public List<KpiInfo> pfsByOccupation;     // List of KPIs for profiles by occupation
        public List<KpiInfo> pfsByRels;           // List of KPIs for profiles by relationships
        public List<KpiInfo> pfsByEdu;            // List of KPIs for profiles by education
    }
}
