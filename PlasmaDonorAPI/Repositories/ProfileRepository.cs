using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Dto;

namespace PlasmaDonorAPI.Repositories
{
    public class ProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileModel> GetByIdAsync(long id)
        {
            return await _context.profiles.FindAsync(id);
        }

        public async Task<ProfileModel> GetByEmailAsync(string email)
        {
            return await _context.profiles.FirstOrDefaultAsync(p => p.email == email);
        }

        public async Task<List<ProfileModel>> GetAllInfluencersByHomeCenterIdAsync(int hcId)
        {
            return await _context.profiles.Where(p => p.isInfluencer==true && p.homeCenterId == hcId).ToListAsync();
        }

        public async Task<List<ProfileModel>> GetAllInfluencersAsync()
        {
            return await _context.profiles.Where(p => p.isInfluencer == true).ToListAsync();
        }

        public async Task<List<ProfileModel>> GetAllProfilesAsync()
        {
            return await _context.profiles
                .Where(p => p.Status == null || p.Status != "deleted")
                .Include(p => p.Language)
                .Include(p => p.Race)
                .Include(p => p.Relationship)
                .Include(p => p.Occupation)
                .Include(p => p.Education)
                .Include(p => p.Address)
                .Include(p => p.HomeCenter)
                .Include(p => p.hobbies)
                .Include(p => p.interests)
                .Include(p => p.InfluencedBy)
                .Include(p => p.isDonor)
                .ToListAsync();
        }

        public async Task<List<ProfileModel>> GetAllInfluencersDetailedAsync()
        {
            return await _context.profiles
                .Where(p => p.isInfluencer == true && (p.Status == null || p.Status != "deleted"))
                .Include(p => p.Language)
                .Include(p => p.Race)
                .Include(p => p.Relationship)
                .Include(p => p.Occupation)
                .Include(p => p.Education)
                .Include(p => p.Address)
                .Include(p => p.HomeCenter)
                .Include(p => p.hobbies)
                .Include(p => p.interests)
                .Include(p => p.InfluencedBy)
                .Include(p => p.isDonor)
                .ToListAsync();
        }

        public async Task SoftDeleteByIdAsync(long id)
        {
            var profile = await _context.profiles.FindAsync(id);
            if (profile != null)
            {
                profile.Status = "deleted";
                await _context.SaveChangesAsync();
            }
        }
    }
}

//using Microsoft.EntityFrameworkCore;
//using NewPlasmaDonorsAPI.Data;
//using NewPlasmaDonorsAPI.Models;
//using NewPlasmaDonorsAPI.Dto;
//using System;

//namespace NewPlasmaDonorsAPI.Repositories
//{
//    public class ProfileRepository
//    {
//        private readonly AppDbContext _context;

//        public ProfileRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public List<Tuple<string, string, string, string, string, bool, bool, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, long>> FindAllProfiles()
//        {
//            return _context.Database.SqlQueryRaw<Tuple<string, string, string, string, string, bool, bool, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, long>>(@"
//                    SELECT a.email, a.first_name, a.last_name, a.phone_number, a.gender, a.dob,   
//                    a.is_donor, a.is_influencer, a.created_on, a.school_attended,  
//                    lang.id, lang.md_title,  
//                    race.id, race.md_title,  
//                    relationship.id, relationship.md_title,  
//                    occupation.id, occupation.md_title,  
//                    education.id, education.md_title,  
//                    addr.id, addr.address_line, addr.city, addr.state, addr.state_code,  
//                    addr.country, addr.country_code, addr.latitude, addr.longitude,   
//                    addr.full_address, addr.postal_code,  
//                    companyLocation.id, companyLocation.site_id,  
//                    a.relship_status, a.id  
//                    FROM profiles a  
//                    LEFT JOIN master_data lang ON a.language_id = lang.id  
//                    LEFT JOIN master_data race ON a.race_id = race.id  
//                    LEFT JOIN master_data relationship ON a.relationship_id = relationship.id  
//                    LEFT JOIN master_data occupation ON a.occupation_id = occupation.id  
//                    LEFT JOIN master_data education ON a.education_id = education.id  
//                    LEFT JOIN company_locations companyLocation ON a.home_center_id = companyLocation.id  
//                    LEFT JOIN address addr ON a.address_id = addr.id  
//                    WHERE a.id IS NOT NULL AND (a.status IS NULL OR a.status <> 'deleted')  
//                    GROUP BY a.id").ToList();
//        }

//        public ProfileModel FindById(long id)
//        {
//            return _context.profiles.FirstOrDefault(p => p.id == id);
//        }

//        public ProfileModel FindByEmail(string email)
//        {
//            return _context.profiles.FirstOrDefault(p => p.email == email);
//        }

//        public List<ProfileModel> FindAllInfluencersByHomeCenterId(int hcId)
//        {
//            return _context.profiles
//                .Where(p => p.isInfluencer == true && p.homeCenterId == hcId)
//                .ToList();
//        }
//        public List<ProfileModel> FindAllInfluencers()
//        {
//            return _context.profiles?
//                .Where(p => p.isInfluencer == true) // Filter profiles where IsInfluencer is true
//                .ToList() ?? new List<ProfileModel>();
//        }
//        public List<ProfileModel> FindAll()
//        {
//            return _context.profiles?.ToList() ?? new List<ProfileModel>();
//        }

//        public async Task<List<ProfileModel>> FindAllAsync()
//        {
//            return await (_context.profiles?.ToListAsync() ?? Task.FromResult(new List<ProfileModel>()));
//        }

//        public void SoftDeleteById(long id)
//        {
//            var profile = _context.profiles.FirstOrDefault(p => p.id == id);
//            if (profile != null)
//            {
//                profile.Status = "deleted";  // Assuming 'Status' is a string column
//                _context.SaveChanges();
//            }
//        }
//    }
//}
