using System.Text;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
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
            if (profileReq == null)
            {
                profileReq = new ProfileDto();
            }

            var query = @"
            SELECT a.email, a.first_name, a.last_name, a.phone_number, a.gender, a.dob,   
                   a.is_donor, a.is_influencer, a.created_on, a.school_attended,  
                   lang.id, lang.md_title,  
                   race.id, race.md_title,  
                   relationship.id, relationship.md_title,  
                   occupation.id, occupation.md_title,  
                   education.id, education.md_title,  
                   addr.id, addr.address_line, addr.city, addr.state, addr.state_code,  
                   addr.country, addr.country_code, addr.latitude, addr.longitude,   
                   addr.full_address, addr.postal_code,  
                   STRING_AGG(DISTINCT influencer.email, ',') AS influencedByList,  
                   STRING_AGG(DISTINCT donors.email, ',') AS donorList,  
                   SUM(infRelationship.md_score) AS infScore,  
                   STRING_AGG(DISTINCT hobbies.md_title, ',') AS hobbiesList,  
                   STRING_AGG(DISTINCT interest.md_title, ',') AS interestList,  
                   STRING_AGG(DISTINCT influencer.id, ',') AS influencedByIds,  
                   STRING_AGG(DISTINCT hobbies.id, ',') AS hobbieIds,  
                   STRING_AGG(DISTINCT interest.id, ',') AS interestIds,  
                   companyLocation.id, companyLocation.site_id,  
                   a.relship_status, a.id  
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
            LEFT JOIN master_data hobbies ON hobbiesMap.md_id = hobbies.id AND hobbies.md_type = 'hobbies'  
            LEFT JOIN profile_md_map interestMap ON a.id = interestMap.profile_id  
            LEFT JOIN master_data interest ON interestMap.md_id = interest.id AND interest.md_type = 'interests'  
            WHERE a.id IS NOT NULL ";

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
            if (profileReq.influencedById > 0)
            {
                sb.Append($" AND influencer.id = {profileReq.influencedById}");
            }

            if (profileReq.relationshipId.HasValue)
            {
                sb.Append($" AND a.relationship_id = {profileReq.relationshipId.Value}");
            }

            if (!string.IsNullOrEmpty(profileReq.gender))
            {
                sb.Append($" AND a.gender = '{profileReq.gender}'");
            }

            if (profileReq.ageGroup > 0)
            {
                var sqlUtilService = new SqlUtilService();
                sb.Append($" AND {sqlUtilService.AgeGroupQuery(profileReq.ageGroup)}");
            }

        
            var results = _context.Database.SqlQuery<ProfileDto>($"{sb.ToString()}").ToList();

            return new ResInfo
            {
                Status = true,
                Data = results
            };
        }

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
            if (hcId.HasValue  && hcId < 0)
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
                ?  _profileRepository.getAllInfluencers()
                    :  _profileRepository.findAllProfiles();
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
            existingProfile.education = updatedModel.education;
            existingProfile.RelshipReason = updatedModel.RelshipReason;
            existingProfile.Relationship = updatedModel.Relationship;
            existingProfile.occupation = updatedModel.occupation;
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

