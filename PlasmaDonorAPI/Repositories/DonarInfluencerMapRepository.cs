using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.Repositories
{
    public class DonarInfluencerMapRepository
    {
        private readonly AppDbContext _context;

        public DonarInfluencerMapRepository(AppDbContext context)
        {
            _context = context;
        }

        // Find all influencer mappings by profile ID
        public List<DonarInfluencerMap> FindAllByProfileId(long profileId)
        {
            return _context.DonarInfluencerMaps?
                .Where(d => d.profileId == profileId)
                .Include(d => d.InfluencerProfile)
                .ToList() ?? new List<DonarInfluencerMap>();
        }

        // Delete influencer mapping by profile and influencer ID
        public void DeleteByInfluencerId(long profileId, long influencerId)
        {
            var query = "DELETE FROM donar_influencer_map WHERE profile_id = {0} AND influenced_by = {1}";
            _context.Database.ExecuteSqlRaw(query, profileId, influencerId);
        }

        // Save a new influencer mapping
        public void Save(DonarInfluencerMap map)
        {
            _context.DonarInfluencerMaps?.Add(map);
            _context.SaveChanges();
        }
    }
}
