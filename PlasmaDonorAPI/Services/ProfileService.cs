using System.Text;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Jose;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.mapper;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.utils;
using PlasmaDonorAPI.Repositories;

namespace NewPlasmaDonorsAPI.Services
{
    public class ProfileService : BaseService
    {
        private readonly AppDbContext _context;
        private readonly DonarInfluencerMapRepository _dimRepo;
        private readonly MdService _mdService;
        private readonly ProfileRepository _profileRepository;

        public ProfileService(
            AppDbContext context,
            DonarInfluencerMapRepository dimRepo,
            MdService mdService,
            ILogger<ProfileService> logger,
            IHttpContextAccessor httpContextAccessor,
            ProfileRepository profileRepository) : base(logger, httpContextAccessor)// Fix: Pass logger to base constructor
        {
            _context = context;
            _dimRepo = dimRepo;
            _mdService = mdService;
            _profileRepository = profileRepository;
        }

        public ResInfo SearchProfiles(ProfileDto profileReq)
        {
            try
            {

                if (profileReq == null)
                {
                    profileReq = new ProfileDto();
                }

                var query = @"
        SELECT a.email, a.first_name, a.last_name, a.phone_number, a.gender, a.dob,   
               a.is_donor, a.is_influencer, a.created_on, a.school_attended,  
               lang.id AS languageId, lang.md_title AS language,  
               race.id AS raceId, race.md_title AS race,  
               relationship.id AS relationshipId, relationship.md_title AS relationship,  
               occupation.id AS occupationId, occupation.md_title AS occupation,  
               education.id AS educationId, education.md_title AS education,  
               addr.id AS addressId, addr.address_line, addr.city, addr.state, addr.state_code,  
               addr.country, addr.country_code, addr.latitude, addr.longitude,   
               addr.full_address, addr.postal_code,  
               companyLocation.id AS homeCenterId, companyLocation.site_id,  
               a.relship_status, a.id  
        FROM profiles a  
        LEFT JOIN master_data lang ON a.language_id = lang.id  
        LEFT JOIN master_data race ON a.race_id = race.id  
        LEFT JOIN master_data relationship ON a.relationship_id = relationship.id  
        LEFT JOIN master_data occupation ON a.occupation_id = occupation.id  
        LEFT JOIN master_data education ON a.education_id = education.id  
        LEFT JOIN company_locations companyLocation ON a.home_center_id = companyLocation.id  
        LEFT JOIN address addr ON a.address_id = addr.id  
        WHERE a.id IS NOT NULL  
    ";

                var sb = new StringBuilder(query);

                // **Filtering Conditions**
                if (profileReq.isDonor.HasValue && profileReq.isDonor.Value)
                {
                    sb.Append(" AND a.is_donor = 1 ");
                }

                if (profileReq.isInfluencer.HasValue && profileReq.isInfluencer.Value)
                {
                    sb.Append(" AND a.is_influencer = 1 ");
                }

                if (profileReq.homeCenterId > 0)
                {
                    sb.Append($" AND a.home_center_id = {profileReq.homeCenterId}");
                }

                if (profileReq.relationshipId.HasValue)
                {
                    sb.Append($" AND a.relationship_id = {profileReq.relationshipId.Value}");
                }

                if (!string.IsNullOrEmpty(profileReq.gender))
                {
                    sb.Append($" AND a.gender = '{profileReq.gender}'");
                }

                sb.Append(" GROUP BY a.id");

                // **Execute SQL Using ADO.NET**
                var profiles = new List<ProfileDto>();

                using (var connection = _context.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sb.ToString();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                profiles.Add(new ProfileDto
                                {
                                    email = reader["email"].ToString(),
                                    firstName = reader["first_name"].ToString(),
                                    lastName = reader["last_name"].ToString(),
                                    name = $"{reader["first_name"]} {reader["last_name"]}",
                                    phoneNumber = reader["phone_number"].ToString(),
                                    gender = reader["gender"].ToString(),
                                    dob = reader["dob"] != DBNull.Value ? (DateTime?)reader["dob"] : null,
                                    isDonor = reader["is_donor"] != DBNull.Value ? Convert.ToBoolean(reader["is_donor"]) : (bool?)null,
                                    isInfluencer = reader["is_influencer"] != DBNull.Value ? Convert.ToBoolean(reader["is_influencer"]) : (bool?)null,
                                    createdOn = reader["created_on"] != DBNull.Value ? (DateTime?)reader["created_on"] : null,
                                    schoolAttended = reader["school_attended"].ToString(),
                                    languageId = reader["languageId"] != DBNull.Value ? (long?)reader["languageId"] : null,
                                    language = reader["language"].ToString(),
                                    raceId = reader["raceId"] != DBNull.Value ? (long?)reader["raceId"] : null,
                                    race = reader["race"].ToString(),
                                    relationshipId = reader["relationshipId"] != DBNull.Value ? (long?)reader["relationshipId"] : null,
                                    relationship = reader["relationship"].ToString(),
                                    occupationId = reader["occupationId"] != DBNull.Value ? (long?)reader["occupationId"] : null,
                                    occupation = reader["occupation"].ToString(),
                                    educationId = reader["educationId"] != DBNull.Value ? (long?)reader["educationId"] : null,
                                    education = reader["education"].ToString(),
                                    addressId = reader["addressId"] != DBNull.Value ? (long?)reader["addressId"] : null,
                                    city = reader["city"].ToString(),
                                    state = reader["state"].ToString(),
                                    country = reader["country"].ToString(),
                                    latitude = reader["latitude"] != DBNull.Value ? (double?)reader["latitude"] : null,
                                    longitude = reader["longitude"] != DBNull.Value ? (double?)reader["longitude"] : null,
                                    fullAddress = reader["full_address"].ToString(),
                                    postalCode = reader["postal_code"].ToString(),
                                    homeCenterId = reader["homeCenterId"] != DBNull.Value ? (long?)reader["homeCenterId"] : null,
                                    relshipStatus = reader["relship_status"].ToString(),
                                    id = (long)reader["id"]
                                });
                            }
                        }
                    }
                }

                return new ResInfo
                {
                    Status = true,
                    Data = profiles
                };
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        //        public ResInfo SearchProfiles(ProfileDto profileReq)
        //        {
        //            if (profileReq == null)
        //            {
        //                profileReq = new ProfileDto();
        //            }

        //            var query = @"
        //SELECT a.email, a.first_name, a.last_name, a.phone_number, a.gender, a.dob,   
        //       a.is_donor, a.is_influencer, a.created_on, a.school_attended,  
        //       lang.id AS languageId, lang.md_title AS language,  
        //       race.id AS raceId, race.md_title AS race,  
        //       relationship.id AS relationshipId, relationship.md_title AS relationship,  
        //       occupation.id AS occupationId, occupation.md_title AS occupation,  
        //       education.id AS educationId, education.md_title AS education,  
        //       addr.id AS addressId, addr.address_line, addr.city, addr.state, addr.state_code,  
        //       addr.country, addr.country_code, addr.latitude, addr.longitude,   
        //       addr.full_address, addr.postal_code,  
        //       STRING_AGG(DISTINCT influencer.email, ',') AS influencedByList,  
        //       STRING_AGG(DISTINCT donors.email, ',') AS donorList,  
        //       SUM(infRelationship.md_score) AS infScore,  
        //       STRING_AGG(DISTINCT hobbies.md_title, ',') AS hobbiesList,  
        //       STRING_AGG(DISTINCT interest.md_title, ',') AS interestList,  
        //       STRING_AGG(DISTINCT influencer.id, ',') AS influencedByIds,  
        //       STRING_AGG(DISTINCT hobbies.id, ',') AS hobbieIds,  
        //       STRING_AGG(DISTINCT interest.id, ',') AS interestIds,  
        //       companyLocation.id AS homeCenterId, companyLocation.site_id,  
        //       a.relship_status, a.id  
        //FROM profiles a  
        //LEFT JOIN master_data lang ON a.language_id = lang.id  
        //LEFT JOIN master_data race ON a.race_id = race.id  
        //LEFT JOIN master_data relationship ON a.relationship_id = relationship.id  
        //LEFT JOIN master_data occupation ON a.occupation_id = occupation.id  
        //LEFT JOIN master_data education ON a.education_id = education.id  
        //LEFT JOIN company_locations companyLocation ON a.home_center_id = companyLocation.id  
        //LEFT JOIN address addr ON a.address_id = addr.id  
        //LEFT JOIN donar_influencer_map influencerMap ON a.id = influencerMap.profile_id  
        //LEFT JOIN profiles influencer ON influencerMap.influenced_by = influencer.id  
        //LEFT JOIN master_data infRelationship ON influencer.relationship_id = infRelationship.id  
        //LEFT JOIN donar_influencer_map donorMap ON donorMap.influenced_by = a.id  
        //LEFT JOIN profiles donors ON donors.id = donorMap.profile_id  
        //LEFT JOIN profile_md_map hobbiesMap ON a.id = hobbiesMap.profile_id  
        //LEFT JOIN master_data hobbies ON hobbiesMap.md_id = hobbies.id AND hobbies.md_type = 'hobbies'  
        //LEFT JOIN profile_md_map interestMap ON a.id = interestMap.profile_id  
        //LEFT JOIN master_data interest ON interestMap.md_id = interest.id AND interest.md_type = 'interests'  
        //WHERE a.id IS NOT NULL  
        //";

        //            var sb = new StringBuilder(query);

        //            if (profileReq.isDonor.HasValue && profileReq.isDonor.Value)
        //            {
        //                sb.Append(" AND a.is_donor = 1 ");
        //            }

        //            if (profileReq.isInfluencer.HasValue && profileReq.isInfluencer.Value)
        //            {
        //                sb.Append(" AND a.is_influencer = 1 ");
        //            }

        //            if (profileReq.homeCenterId > 0)
        //            {
        //                sb.Append(" AND a.home_center_id = @homeCenterId");
        //            }
        //            if (profileReq.influencedById > 0)
        //            {
        //                sb.Append(" AND influencer.id = @influencedById");
        //            }

        //            if (profileReq.relationshipId.HasValue)
        //            {
        //                sb.Append(" AND a.relationship_id = @relationshipId");
        //            }

        //            if (!string.IsNullOrEmpty(profileReq.gender))
        //            {
        //                sb.Append(" AND a.gender = @gender");
        //            }

        //            sb.Append(" GROUP BY a.id");

        //            var results = _context.Database.FromSqlRaw(sb.ToString(),
        //                    new SqlParameter("@homeCenterId", profileReq.homeCenterId),
        //                    new SqlParameter("@influencedById", profileReq.influencedById),
        //                    new SqlParameter("@relationshipId", profileReq.relationshipId ?? (object)DBNull.Value),
        //                    new SqlParameter("@gender", profileReq.gender ?? (object)DBNull.Value)
        //                ).ToList();

        //            var profiles = results.Select(p => new ProfileDto
        //            {
        //                email = p.email,
        //                firstName = p.firstName,
        //                lastName = p.lastName,
        //                name = $"{p.firstName} {p.lastName}",
        //                phoneNumber = p.phoneNumber,
        //                gender = p.gender,
        //                dob = p.dob ?? DateTime.MinValue,
        //                isDonor = p.isDonor,
        //                isInfluencer = p.isInfluencer,
        //                createdOn = p.createdOn ?? DateTime.MinValue,
        //                schoolAttended = p.schoolAttended,
        //                languageId = p.languageId,
        //                language = p.language,
        //                raceId = p.raceId,
        //                race = p.race,
        //                relationshipId = p.relationshipId,
        //                relationship = p.relationship,
        //                occupationId = p.occupationId,
        //                occupation = p.occupation,
        //                educationId = p.educationId,
        //                education = p.education,
        //                addressId = p.addressId,
        //                addressLine1 = p.addressLine1,
        //                city = p.city,
        //                state = p.state,
        //                stateCode = p.stateCode,
        //                country = p.country,
        //                countryCode = p.countryCode,
        //                latitude = p.latitude,
        //                longitude = p.longitude,
        //                fullAddress = p.fullAddress,
        //                postalCode = p.postalCode,
        //                influencerIds = p.influencedByList?.Split(',').Select(long.Parse).ToList(),
        //                hobbiesIds = p.hobbiesList?.Split(',').Select(long.Parse).ToList(),
        //                interestIds = p.interestList?.Split(',').Select(long.Parse).ToList()
        //            }).ToList();

        //            return new ResInfo
        //            {
        //                Status = true,
        //                Data = profiles
        //            };
        //        }


        //        public ResInfo SearchProfiles(ProfileDto profileReq)
        //        {
        //            if (profileReq == null)
        //            {
        //                profileReq = new ProfileDto();
        //            }

        //            var query = @"
        //SELECT a.email, a.first_name, a.last_name, a.phone_number, a.gender, a.dob,   
        //       a.is_donor, a.is_influencer, a.created_on, a.school_attended,  
        //       lang.id, lang.md_title,  
        //       race.id, race.md_title,  
        //       relationship.id, relationship.md_title,  
        //       occupation.id, occupation.md_title,  
        //       education.id, education.md_title,  
        //       addr.id, addr.address_line, addr.city, addr.state, addr.state_code,  
        //       addr.country, addr.country_code, addr.latitude, addr.longitude,   
        //       addr.full_address, addr.postal_code,  
        //       GROUP_CONCAT(DISTINCT influencer.email SEPARATOR ',') AS influencedByList,  
        //       GROUP_CONCAT(DISTINCT donors.email SEPARATOR ',') AS donorList,  
        //       SUM(infRelationship.md_score) AS infScore,  
        //       GROUP_CONCAT(DISTINCT hobbies.md_title SEPARATOR ',') AS hobbiesList,  
        //       GROUP_CONCAT(DISTINCT interest.md_title SEPARATOR ',') AS interestList,  
        //       GROUP_CONCAT(DISTINCT influencer.id SEPARATOR ',') AS influencedByIds,  
        //       GROUP_CONCAT(DISTINCT hobbies.id SEPARATOR ',') AS hobbieIds,  
        //       GROUP_CONCAT(DISTINCT interest.id SEPARATOR ',') AS interestIds,  
        //       companyLocation.id, companyLocation.site_id,  
        //       a.relship_status, a.id  
        //FROM profiles a  
        //LEFT JOIN master_data lang ON a.language_id = lang.id  
        //LEFT JOIN master_data race ON a.race_id = race.id  
        //LEFT JOIN master_data relationship ON a.relationship_id = relationship.id  
        //LEFT JOIN master_data occupation ON a.occupation_id = occupation.id  
        //LEFT JOIN master_data education ON a.education_id = education.id  
        //LEFT JOIN company_locations companyLocation ON a.home_center_id = companyLocation.id  
        //LEFT JOIN address addr ON a.address_id = addr.id  
        //LEFT JOIN donar_influencer_map influencerMap ON a.id = influencerMap.profile_id  
        //LEFT JOIN profiles influencer ON influencerMap.influenced_by = influencer.id  
        //LEFT JOIN master_data infRelationship ON influencer.relationship_id = infRelationship.id  
        //LEFT JOIN donar_influencer_map donorMap ON donorMap.influenced_by = a.id  
        //LEFT JOIN profiles donors ON donors.id = donorMap.profile_id  
        //LEFT JOIN profile_md_map hobbiesMap ON a.id = hobbiesMap.profile_id  
        //LEFT JOIN master_data hobbies ON hobbiesMap.md_id = hobbies.id AND hobbies.md_type = 'hobbies'  
        //LEFT JOIN profile_md_map interestMap ON a.id = interestMap.profile_id  
        //LEFT JOIN master_data interest ON interestMap.md_id = interest.id AND interest.md_type = 'interests'  
        //WHERE a.id IS NOT NULL  
        //"; 


        //            var sb = new StringBuilder(query);

        //            // **Filtering Conditions**
        //            if (profileReq.isDonor.HasValue && profileReq.isDonor.Value)
        //            {
        //                sb.Append(" AND a.is_donor = 1 ");
        //            }

        //            if (profileReq.isInfluencer.HasValue && profileReq.isInfluencer.Value)
        //            {
        //                sb.Append(" AND a.is_influencer = 1 ");
        //            }

        //            if (profileReq.homeCenterId > 0)
        //            {
        //                sb.Append($" AND a.home_center_id = {profileReq.homeCenterId}");
        //            }
        //            if (profileReq.influencedById > 0)
        //            {
        //                sb.Append($" AND influencer.id = {profileReq.influencedById}");
        //            }

        //            if (profileReq.relationshipId.HasValue)
        //            {
        //                sb.Append($" AND a.relationship_id = {profileReq.relationshipId.Value}");
        //            }

        //            if (!string.IsNullOrEmpty(profileReq.gender))
        //            {
        //                sb.Append($" AND a.gender = '{profileReq.gender}'");
        //            }

        //            if (profileReq.ageGroup > 0)
        //            {
        //                var sqlUtilService = new SqlUtilService();
        //                sb.Append($" AND {sqlUtilService.AgeGroupQuery(profileReq.ageGroup)}");
        //            }

        //            sb.Append("GROUP BY a.id");

        //            var results = _context.Database.SqlQuery<ProfileDto>($"{sb.ToString()}").ToList();
        //            var profiles = results.Select(profile => new ProfileDto
        //            {
        //                email = profile.email,
        //                firstName = profile.firstName,
        //                lastName = profile.lastName,
        //                name = $"{profile.firstName} {profile.lastName}",
        //                phoneNumber = profile.phoneNumber,
        //                gender = profile.gender,
        //                dob = profile.dob,  // Handle nullable DateTime
        //                isDonor = profile.isDonor,
        //                isInfluencer = profile.isInfluencer,
        //                createdOn = profile.createdOn,  // Handle nullable DateTime
        //                schoolAttended = profile.schoolAttended,
        //                languageId = profile.languageId,
        //                language = profile.language,
        //                raceId = profile.raceId,
        //                race = profile.race,
        //                relationshipId = profile.relationshipId,
        //                relationship = profile.relationship,
        //                occupationId = profile.occupationId,
        //                occupation = profile.occupation,
        //                educationId = profile.educationId,
        //                education = profile.education,
        //                addressId = profile.addressId,
        //                addressLine1 = profile.addressLine1,
        //                city = profile.city,
        //                state = profile.state,
        //                stateCode = profile.stateCode,
        //                country = profile.country,
        //                countryCode = profile.countryCode,
        //                latitude = profile.latitude,
        //                longitude = profile.longitude,
        //                fullAddress = profile.fullAddress,
        //                postalCode = profile.postalCode,
        //                influencers = profile.influencers,
        //                infScore = profile.infScore,
        //                hobbieStr = profile.hobbieStr,
        //                interestStr = profile.interestStr,
        //                homeCenterId = profile.homeCenterId,
        //                homeCenter = profile.homeCenter,
        //                relshipStatus = profile.relshipStatus,
        //                id = profile.id,
        //            }).ToList();

        //            // **Parse IDs from CSV Strings**
        //            foreach (var profile in profiles)
        //            {
        //                var infIds = profile.influencers;
        //                var hobbies = profile.hobbieStr;
        //                var interests = profile.interestStr;

        //                if (!string.IsNullOrEmpty(infIds))
        //                {
        //                    profile.influencerIds = infIds.Split(',').Select(i => long.Parse(i.Trim())).ToList();
        //                }

        //                if (!string.IsNullOrEmpty(hobbies))
        //                {
        //                    profile.hobbiesIds = hobbies.Split(',').Select(i => long.Parse(i.Trim())).ToList();
        //                }

        //                if (!string.IsNullOrEmpty(interests))
        //                {
        //                    profile.interestIds = interests.Split(',').Select(i => long.Parse(i.Trim())).ToList();
        //                }
        //            }


        //            return new ResInfo
        //            {
        //                Status = true,
        //                Data = results
        //            };
        //        }

        public ResInfo AddProfile(ProfileDto info)
        {
            if (string.IsNullOrEmpty(info.email))
            {
                return Error("Invalid data: email is invalid");
            }

            var existedProfile = _context.profiles?.FirstOrDefault(p => p.email == info.email);
            if (existedProfile != null)
            {
                return Error("Email already existed");
            }

            var profileModel = ProfileMapper.MapToProfileModel(info);
            profileModel!.createdOn = DateTime.Now;
            MasterData? data = null!;

            if (info.languageId.HasValue)
            {
                data = _mdService.GetMdEntry(info.languageId.Value);
                if (data != null)
                {
                    profileModel.Language = data;
                }
            }

            if (info.raceId.HasValue)
            {
                data = _mdService.GetMdEntry(info.raceId.Value);
                if (data != null)
                {
                    profileModel.Race = data;
                }
            }

            if (info.relationshipId.HasValue)
            {
                data = _mdService.GetMdEntry(info.relationshipId.Value);
                if (data != null)
                {
                    profileModel.Relationship = data;
                }
            }

            if (info.occupationId.HasValue)
            {
                data = _mdService.GetMdEntry(info.occupationId.Value);
                if (data != null)
                {
                    profileModel.Occupation = data;
                }
            }

            ProfileModel savedProfile;
            try
            {
                _context.profiles!.Add(profileModel);
                _context.SaveChanges();
                savedProfile = profileModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving profile to the database");
                return Error("Error while saving into Database: " + e.Message);
            }

            long profileId = savedProfile.id;


            if (info.influencerIds != null && info.influencerIds.Any())
            {
                // Get all existing influencers for this profile
                var existingInfluencers = _dimRepo.FindAllByProfileId(profileId);
                var infIds = existingInfluencers.Select(mp => mp.InfluencerProfile!.id).ToList();

                foreach (var id in info.influencerIds)
                {
                    // Check if the influencer profile exists
                    var influencedProfile = _context.profiles.Find(id);
                    if (influencedProfile == null)
                    {
                        _logger.LogWarning("Influenced profile with ID {id} not found. Skipping.", id);
                        continue; // Skip invalid influencer IDs
                    }

                    if (infIds.Contains(id))
                    {
                        // If the influencer already exists, remove it from the list
                        infIds.Remove(id);
                    }
                    else
                    {
                        // Add a new mapping for the influencer
                        var influencer = new DonarInfluencerMap
                        {
                            profileId = savedProfile.id,      // Use the saved profile's ID (foreign key)
                            influencedBy = id  // Assign the navigation property
                        };
                        _dimRepo.Save(influencer);
                    }
                }
                if (infIds.Any())
                {
                    foreach (var infId in infIds)
                    {
                        _dimRepo.DeleteByInfluencerId(profileId, infId);
                    }
                }
            }

            return Success(savedProfile);
        }

        public ResInfo GetInfluencersForLb(int? hcId)
        {
            _logger.LogInformation("Getting influencers for LB.");

            List<ProfileModel> list = new List<ProfileModel>();
            //var list=new List<ProfileModel>();
            if (hcId.HasValue && hcId < 0)
            {
                hcId = null;
            }

            if (hcId.HasValue)
            {
                list = _profileRepository.GetAllInfluencersByHomeCenterIdAsync(hcId.Value).Result;
            }
            else
            {
                list = _profileRepository.FindAllInfluencers();
            }

            var influencers = list.Select(l => new MdInfo
            {
                Id = l.id,
                Name = l.firstName + " " + l.lastName
            }).ToList();

            return Success(influencers);
        }

        public async Task<ResInfo> GetProfileListNew()
        {
            return await GetProfileListNew(-1L, null);
        }

        public async Task<ResInfo> GetProfileListNew(long? hcmId = null, bool? isInfluencer = null)
        {
            _logger.LogInformation("Fetching profile list with filters: hcmId={hcmId}, isInfluencer={isInfluencer}", hcmId, isInfluencer);

            IEnumerable<dynamic> list;
            if (hcmId.HasValue && hcmId > 0)
            {
                list = isInfluencer.HasValue && isInfluencer.Value
                    ? await _profileRepository.GetAllInfluencersByHomeCenterAsync(hcmId.Value)
                    : new List<dynamic>();
            }
            else
            {
                list = isInfluencer.HasValue && isInfluencer.Value
                ? _profileRepository.getAllInfluencers()
                    : _profileRepository.findAllProfiles();
            }
            string email = string.Empty;
            int cnt = 0;
            var profileList = list.Take(10).Select(t =>
            {
                cnt++;
                email = t.email;
                try
                {
                    var profileDto = new ProfileDto
                    {
                        email = t.email,
                        firstName = t.firstName,
                        lastName = t.lastName,
                        phoneNumber = NameUtils.StrVal(t.phoneNumber),
                        name = NameUtils.Appender(" ", NameUtils.StrVal(t.firstName), NameUtils.StrVal(t.lastName)),
                        //gender = NameUtils.Gender(NameUtils.StrVal(t.gender)),
                        //dob = DateUtils.ToShortString(NameUtils.DateVal(t.dob)),
                        //isDonor = NullUtils.IsValid(NameUtils.BoolVal(t.isDonor)) ? NameUtils.BoolVal(t.isDonor) : false,
                        //isInfluencer = NullUtils.IsValid(NameUtils.BoolVal(t.isInfluencer)) ? NameUtils.BoolVal(t.isInfluencer) : false,
                        //createdOn = DateUtils.ToShortString(NameUtils.DateVal(t.createdOn)),
                        //schoolAttended = NameUtils.StrVal(t.schoolAttended),
                        //languageId = NameUtils.LongVal(t.languageId),
                        //language = NameUtils.StrVal(t.language),
                        //raceId = NameUtils.LongVal(t.raceId),
                        //race = NameUtils.StrVal(t.race),
                        //relationshipId = NameUtils.LongVal(t.relationshipId),
                        //relationship = NameUtils.StrVal(t.relationship),
                        //occupationId = NameUtils.LongVal(t.occupationId),
                        //occupation = NameUtils.StrVal(t.occupation),
                        //educationId = NameUtils.LongVal(t.educationId),
                        //education = NameUtils.StrVal(t.education),
                        //addressId = NameUtils.LongVal(t.addressId),
                        //addressLine1 = NameUtils.StrVal(t.addressLine1),
                        //city = NameUtils.StrVal(t.city),
                        //state = NameUtils.StrVal(t.state),
                        //stateCode = NameUtils.StrVal(t.stateCode),
                        //country = NameUtils.StrVal(t.country),
                        //countryCode = NameUtils.StrVal(t.countryCode),
                        //latitude = NameUtils.DoubleVal(t.latitude),
                        //longitude = NameUtils.DoubleVal(t.longitude),
                        //fullAddress = NameUtils.StrVal(t.fullAddress),
                        //postalCode = NameUtils.StrVal(t.postalCode),
                        //influencers = NameUtils.StrVal(t.influencers),
                        //infScore = NameUtils.DoubleVal(t.infScore),
                        //hobbieStr = NameUtils.StrVal(t.hobbieStr),
                        //interestStr = NameUtils.StrVal(t.interestStr),
                        //homeCenterId = NameUtils.LongVal(t.homeCenterId),
                        //homeCenter = NameUtils.StrVal(t.homeCenter),
                        //relshipStatus = NameUtils.StrVal(t.relshipStatus),
                        id = t.id,
                    };

                    // Convert CSV strings to lists
                    string infIds = NameUtils.StrVal(t.infIds);
                    string hobbies = NameUtils.StrVal(t.hobbies);
                    string interests = NameUtils.StrVal(t.interests);

                    //if (NullUtils.IsValid(infIds))
                    //{
                    //    profileDto.influencerIds = infIds.Split(',')
                    //        .Where(i => !string.IsNullOrWhiteSpace(i))
                    //        .Select(i => long.Parse(i.Trim())).ToList();
                    //}
                    //if (NullUtils.IsValid(hobbies))
                    //{
                    //    profileDto.hobbiesIds = hobbies.Split(',')
                    //        .Where(i => !string.IsNullOrWhiteSpace(i))
                    //        .Select(i => long.Parse(i.Trim())).ToList();
                    //}
                    //if (NullUtils.IsValid(interests))
                    //{
                    //    profileDto.interestIds = interests.Split(',')
                    //        .Where(i => !string.IsNullOrWhiteSpace(i))
                    //        .Select(i => long.Parse(i.Trim())).ToList();
                    //}

                    return profileDto;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error Occurred for " + email + " at " + cnt);

                }
                return null;
            }).ToList();

            return Success(profileList);
        }
        public string UpdateProfile(ProfileModel updatedModel, long id)
        {
            // Find the existing profile by ID
            var existingProfile = _context.profiles?.Find(id);

            if (existingProfile == null)
            {
                throw new Exception("Profile not found"); // Or return an error response
            }

            // Update the fields with values from the updated model
            existingProfile.Education = updatedModel.Education;
            existingProfile.RelshipReason = updatedModel.RelshipReason;
            existingProfile.Relationship = updatedModel.Relationship;
            existingProfile.Occupation = updatedModel.Occupation;
            existingProfile.Language = updatedModel.Language;
            existingProfile.Address = updatedModel.Address;
            existingProfile.Race = updatedModel.Race;

            // Save changes to the database
            _context.SaveChanges();

            return "Profile details updated successfully";
        }

        // Helper method to map MasterData (language, race, occupation, relationship)
        private void MapMasterData(ProfileModel m, ref ProfileDto info)
        {
            MasterData md;

            // Mapping for language
            md = m.Language!;
            if (md != null)
            {
                info.languageId = md.id;
                info.language = md.mdTitle;
            }

            // Mapping for race
            md = m.Race!;
            if (md != null)
            {
                info.raceId = md.id;
                info.race = md.mdTitle;
            }

            // Mapping for occupation
            md = m.Occupation!;
            if (md != null)
            {
                info.occupationId = md.id;
                info.occupation = md.mdTitle;
            }

            // Mapping for relationship
            md = m.Relationship!;
            if (md != null)
            {
                info.relationshipId = md.id;
                info.relationship = md.mdTitle;
            }
        }

        public IActionResult DeleteProfile(long id)
        {
            _profileRepository.SoftDeleteByIdAsync(id);
            var response = new ResInfo { Data = "deleted" };
            return new OkObjectResult(response); // Fix: Use OkObjectResult instead of Ok
        }



    }
}

