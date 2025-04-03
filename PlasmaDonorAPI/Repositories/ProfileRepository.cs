using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.utils;
using NPOI.SS.Formula.Functions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PlasmaDonorAPI.Repositories
{
    public class ProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        string profileQuery = @"
        SELECT a.email, a.first_name, a.last_name, a.phone_number, a.gender, a.dob,  
        a.is_donor, a.is_influencer, a.created_on, a.school_attended,  
        lang.md_title AS Language,  
        race.md_title AS Race,  
        relationship.md_title AS Relationship,  
        occupation.md_title AS Occupation,  
        education.md_title AS Education,  
        addr.address_line AS Address, addr.city, addr.state, addr.country,  
        addr.latitude, addr.longitude, addr.postal_code,  
        GROUP_CONCAT(DISTINCT influencer.email SEPARATOR ',') AS InfluencedByList,  
        GROUP_CONCAT(DISTINCT donors.email SEPARATOR ',') AS DonorList,  
        SUM(infRelationship.md_score) AS InfScore,  
        GROUP_CONCAT(DISTINCT hobbies.md_title SEPARATOR ',') AS HobbiesList,  
        GROUP_CONCAT(DISTINCT interest.md_title SEPARATOR ',') AS InterestList,  
        GROUP_CONCAT(DISTINCT influencer.id SEPARATOR ',') AS InfluencedByIds,  
        GROUP_CONCAT(DISTINCT hobbies.id SEPARATOR ',') AS HobbieIds,  
        GROUP_CONCAT(DISTINCT interest.id SEPARATOR ',') AS InterestIds,  
        companyLocation.id AS CompanyLocationId, companyLocation.site_id AS SiteId,  
        a.relship_status AS RelationshipStatus, a.id AS ProfileId  
        FROM profiles a  
        LEFT JOIN master_data lang ON a.language_id = lang.id  
        LEFT JOIN master_data race ON a.race_id = race.id  
        LEFT JOIN master_data relationship ON a.relationship_id = relationship.id  
        LEFT JOIN master_data occupation ON a.occupation_id = occupation.id  
        LEFT JOIN master_data education ON a.education_id = education.id  
        LEFT JOIN company_locations companyLocation ON a.home_center_id = companyLocation.id  
        LEFT JOIN address addr ON a.address_id = addr.id  
        LEFT JOIN donar_influencer_map influencerMap ON a.id = influencerMap.profile_id  
        LEFT JOIN profiles influencer ON influencerMap.influenced_by = influencer.id  
        LEFT JOIN master_data infRelationship ON influencer.relationship_id = infRelationship.id  
        LEFT JOIN donar_influencer_map donorMap ON donorMap.influenced_by = a.id  
        LEFT JOIN profiles donors ON donors.id = donorMap.profile_id  
        LEFT JOIN profile_md_map hobbiesMap ON a.id = hobbiesMap.profile_id  
        LEFT JOIN master_data hobbies ON hobbiesMap.md_id = hobbies.id AND hobbies.md_type= 'hobbies'  
        LEFT JOIN profile_md_map interestMap ON a.id = interestMap.profile_id  
        LEFT JOIN master_data interest ON interestMap.md_id = interest.id AND interest.md_type= 'interests'  
        WHERE a.id IS NOT NULL AND (a.status IS NULL OR a.status <> 'deleted')";

        String grpBy = "GROUP BY  a.id ";

        public async Task<ProfileModel> GetByIdAsync(long id)
        {
            return await _context.profiles.FindAsync(id);
        }

        public async Task<ProfileModel> GetByEmailAsync(string email)
        {
            return await _context.profiles.FirstOrDefaultAsync(p => p.email == email);
        }

        public List<ProfileModel> FindAllInfluencers()
        {
            return _context.profiles
                .Where(p => p.isInfluencer == true)
                .ToList();
        }

        public async Task<List<ProfileDto>> GetAllInfluencersByHomeCenterAsync(long hcId)
        {
            var query = profileQuery + " AND a.home_center_id = {0} AND a.is_influencer = true " + grpBy;

            var profiles = await _context.profiles
                .FromSqlRaw(query, hcId)
                .Select(p => new ProfileDto
                {
                    email = p.email,
                    firstName = p.firstName,
                    lastName = p.lastName,
                    phoneNumber = p.phoneNumber,
                    isInfluencer = p.isInfluencer,
                    homeCenterId = p.homeCenterId
                })
                .ToListAsync();

            return profiles;
        }

        public async Task<List<ProfileModel>> GetAllInfluencersByHomeCenterIdAsync(int hcId)
        {
            return await _context.profiles?.Where(p => p.isInfluencer == true && p.homeCenterId == hcId).ToListAsync() ?? new List<ProfileModel>();
        }

        public List<ProfileModel> getAllInfluencers()
        {
            return _context.profiles?.Where(p => p.isInfluencer == true).ToList() ?? new List<ProfileModel>();
        }

        //public async Task<List<ProfileModel>> GetAllProfilesAsync()
        //{
        //    string query = profileQuery + grpBy;

        //    var result = await _context.profiles
        //        .FromSqlRaw(query)
        //        .Select(p => new ProfileModel
        //        {
        //            email = p.email,
        //            firstName = p.firstName,
        //            lastName = p.lastName,
        //            phoneNumber = p.phoneNumber,

        //        })
        //        .ToListAsync();

        //    return result;
        //}

        public List<ProfileDto> findAllProfiles()
        {
            List<ProfileDto> profiles = new List<ProfileDto>();

            var conn = _context.Database.GetDbConnection();
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = profileQuery + grpBy;
                    cmd.CommandType = CommandType.Text;

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ProfileDto profile = new ProfileDto
                        {
                            id = reader["ProfileId"] != DBNull.Value ? Convert.ToInt64(reader["ProfileId"]) : (long?)null,
                            firstName = reader["first_name"]?.ToString(),
                            lastName = reader["last_name"]?.ToString(),
                            dob = DateUtils.ToShortString(NameUtils.DateVal(reader["dob"])),
                            gender = reader["gender"]?.ToString(),
                            phoneNumber = reader["phone_number"]?.ToString(),
                            email = reader["email"]?.ToString(),
                            isDonor = reader["is_donor"] != DBNull.Value && Convert.ToBoolean(reader["is_donor"]),
                            isInfluencer = reader["is_influencer"] != DBNull.Value && Convert.ToBoolean(reader["is_influencer"]),
                            createdOn = DateUtils.ToShortString(NameUtils.DateVal(reader["created_on"])),
                            schoolAttended = reader["school_attended"]?.ToString(),

                            language = reader["Language"]?.ToString(),
                            race = reader["Race"]?.ToString(),
                            relationship = reader["Relationship"]?.ToString(),
                            occupation = reader["Occupation"]?.ToString(),
                            education = reader["Education"]?.ToString(),

                            address = reader["Address"]?.ToString(),
                            addressLine1 = reader["Address"]?.ToString(),
                            city = reader["city"]?.ToString(),
                            state = reader["state"]?.ToString(),
                            country = reader["country"]?.ToString(),
                            postalCode = reader["postal_code"]?.ToString(),
                            latitude = reader["latitude"] != DBNull.Value ? Convert.ToDouble(reader["latitude"]) : (double?)null,
                            longitude = reader["longitude"] != DBNull.Value ? Convert.ToDouble(reader["longitude"]) : (double?)null,

                            influencers = reader["InfluencedByList"]?.ToString(),
                            influencedBy = reader["InfluencedByIds"]?.ToString(),
                            interestStr = reader["InterestList"]?.ToString(),
                            hobbieStr = reader["HobbiesList"]?.ToString(),

                            infScore = reader["InfScore"] != DBNull.Value ? Convert.ToDouble(reader["InfScore"]) : 0,

                            homeCenterId = reader["CompanyLocationId"] != DBNull.Value ? Convert.ToInt64(reader["CompanyLocationId"]) : (long?)null,
                            homeCenter = reader["SiteId"]?.ToString(),

                            relshipStatus = reader["RelationshipStatus"]?.ToString()
                        };
                        profiles.Add(profile);
                    }
                    reader.Close();
                }
            }

            return profiles;
        }


        //public List<ProfileDto> findAllProfiles()

        //{
        //    string query = profileQuery + grpBy;  // Combine the query parts dynamically

        //    return _context.profiles
        //        .FromSqlRaw(query)
        //        .Select(t => new ProfileDto
        //        {
        //            //email=p.email,
        //            //firstName= p.firstName,
        //            //lastName=p.lastName,
        //            //phoneNumber=p.phoneNumber

        //            id = t.id,
        //            phoneNumber = t.phoneNumber,
        //            email = t.email,
        //            firstName = t.firstName,
        //            lastName = t.lastName,
        //            name = NameUtils.Appender(" ", t.firstName, t.lastName),
        //            dob = t.dob,
        //            isDonor = t.isDonor,
        //            isInfluencer = t.isInfluencer,
        //            //age = t.age,
        //            //gender = t.gender,
        //            //city = t.city,
        //            //state = t.state,

        //            createdOn = t.createdOn,
        //            infScore = t.infScore,
        //            schoolAttended = t.schoolAttended,
        //            //ageGroup = null, // Calculate if required
        //            // = null, // Map if available
        //            //children = new List<ProfileDto>() // Handle child profiles if needed
        //            //influencerIds = t.influencedByIds?.Split(',').Select(long.Parse).ToList(),
        //            //influencers = t.InfluencedByList,
        //            //education = t.education,
        //            //educationId = t.educationId,
        //            //realtionshipScore = t.realtionshipScore,
        //            //realtionshipScoreId = t.realtionshipScoreId,

        //            //fullAddress = t.address,
        //            //addressLine1 = t.addressLine,

        //            //stateCode = null, // Map if available
        //            //country = t.country,
        //            //countryCode = null, // Map if available
        //            //postalCode = t.postalCode,
        //            //latitude = t.latitude,
        //            //longitude = t.longitude,

        //            //interests = t.InterestList?.Split(',').ToList(),
        //            //interestStr = t.interestStr,
        //            //interestIds = t.InterestIds?.Split(',').Select(long.Parse).ToList(),
        //            //hobbies = t.HobbiesList?.Split(',').ToList(),
        //            //hobbieStr = t.hobbieStr,
        //            //hobbiesIds = t.HobbieIds?.Split(',').Select(long.Parse).ToList(),
        //            //homeCenterId = t.CompanyLocationId,
        //            //homeCenter = t.homeCenter,

        //            //relshipStatus = t.relationship,
        //            //occupation = t.occupation,
        //            //occupationId = t.occupationId,
        //            //relationship = t.relationship,
        //            //relationshipId = t.relationshipId,
        //            //race = t.race,
        //            //raceId = t.raceId,
        //            //address = t.address,
        //            //addressId = t.addressId,
        //            //language = t.language,
        //            //languageId = t.languageId,
        //            //status = t.status,
        //            //influencedBy = t.influencedBy,

        //        })
        //        .ToList();
        //}
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
                //.Include(p => p.hobbies)
                //.Include(p => p.interests)
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
