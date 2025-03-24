
using System.Globalization;
using Microsoft.VisualStudio.OLE.Interop;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.utils;
using PlasmaDonorAPI.Repositories;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Dto.Dashboard;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.utils;
using Serilog;
using AutoMapper;
using System.Linq;
using System.Text;
using NewPlasmaDonorsAPI.Data;
using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Models;
using Log = Serilog.Log;

namespace NewPlasmaDonorsAPI.Services
{
    public class StatService : BaseService
    {
        private readonly ILogger<StatService> _logger;
        private readonly IStatRepo _statRepo;
        private readonly ProfileRepository _profileRepository;
        private readonly TreeUtils _treeUtils;
        private readonly ProfileService _profileService;
        private readonly SqlUtilService _sqlUtilService;
        private readonly AppDbContext _context;

        // Primary constructor that initializes all dependencies
        public StatService(
            ILogger<StatService> logger,
            IHttpContextAccessor httpContextAccessor,
            IStatRepo statRepo,
            ProfileRepository profileRepository,
            TreeUtils treeUtils,
            ProfileService profileService,
            SqlUtilService sqlUtilService,
            AppDbContext context
        ) : base(logger,httpContextAccessor)
        {
            _logger = logger;
            _statRepo = statRepo;
            _profileRepository = profileRepository;
            _treeUtils = treeUtils;
            _profileService = profileService;
            _sqlUtilService = sqlUtilService;
            _context = context;
        }

        public async Task<ResInfo> GetDashboardStatsAsync()
        {
            // Get donor count
            int donorCount = await _statRepo.GetDonorCountAsync();

            // Get influencer count
            int infCount = await _statRepo.GetInfluencerCountAsync();

            DateTime date30DaysAgo = DateTime.Now.AddDays(-30);

            // Get recent influencer count
            int recentInfCount = await _statRepo.GetRecentInfluencerCountAsync(date30DaysAgo);
            Log.Information(":: Inf Count::" + infCount);

            int recentDonorCount = await _statRepo.GetRecentDonorCountAsync(date30DaysAgo);
            Log.Information(":: donor Count::" + donorCount);

            var dbsInfo = new DashboardStatInfo
            {
                topCards = new List<KpiInfo>
                {
                    new KpiInfo { bgColor = "success", icon = "bi bi-wallet", title = "Donors", valStr = donorCount.ToString() },
                    new KpiInfo { bgColor = "danger", icon = "bi bi-brightness-high", title = "Influencers", valStr = infCount.ToString() },
                    new KpiInfo { bgColor = "warning", icon = "bi bi-book-half", title = "New Donors", valStr = recentDonorCount.ToString() },
                    new KpiInfo { bgColor = "info", icon = "bi bi-box-fill", title = "New Influencers", valStr = recentInfCount.ToString() }
                }
            };
           
            List<Tuple<string, int>> lst = _statRepo.GetInfTimeSeries(DateTime.Now);
            // Convert first dataset and set to InfSeries
            dbsInfo.infSeries = ToKpiInfo(lst);

            // Get influencer's time series by date
            lst = _statRepo.GetDonorTimeSeries(DateTime.Now);
            dbsInfo.donorSeries = ToKpiInfo(lst);

            // Get profiles by state
            var profilesByState = _statRepo.GetProfilesByState();
            dbsInfo.pfsByStates = ToKpiInfoByLookup(profilesByState);

            // Get donors by occupation
            var donorsByOccupation = _statRepo.GetDonorsByOccupation();
            dbsInfo.pfsByOccupation = ToKpiInfo(donorsByOccupation);
            //dbsInfo.pfsByOccupation = ToKpiInfoByLookup(lst);

           

          //PENDING BELOW METHOD
            //List<TopInfluencerInfo> topInfs = await _statRepo.GetTopInfluencersAsync();

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<TopInfluencerInfo, TopInfluencersinfo>();
            //});
            //var mapper = config.CreateMapper();
            //// dbsInfo.topInfluencers = mapper.Map<List<TopInfluencersinfo>>(topInfs);
            //dbsInfo.topInfluencers = topInfs
            // .Select(t => new TopInfluencerInfo
            // {
            //     Name = t.Item1,   // Name from Tuple<string, double>
            //     Count = t.Item2,  // Count from Tuple<string, double>
            //     Email = "",       // Default value (update if needed)
            //     Location = "",    // Default value (update if needed)
            //     Icn = "",         // Default value (update if needed)
            //     CssCls = "success"
            // })
            // .ToList();

            //dbsInfo.topInfluencers = topInfs.Select(t =>
            //{
            //    // Ensure name is properly split
            //    string firstName = t.Name?.Split(' ').FirstOrDefault() ?? "";
            //    string lastName = t.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "";

            //    // Extract initials (icnTxt)
            //    string iconTxt = lastName.Length >= 2 ? lastName.Substring(0, 2) : "??";
            //    if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            //    {
            //        iconTxt = $"{firstName[0]}{lastName[0]}";
            //    }

            //    return new TopInfluencerInfo
            //    {
            //        Name = $"{firstName} {lastName}".Trim(),
            //        Email = t.Email,
            //        //Location = $"{t.AddressLine}, {t.City}, {t.State}".Trim(),
            //        Location = $"{t.Location}".Trim(),
            //        Count = t.Count,
            //        Icn = iconTxt.ToUpper(),
            //        CssCls = "success"
            //    };
            //}).ToList();


            var statsDto = new NewPlasmaDonorsAPI.Dto.DashboardStatsDto
            {
                DonorCount = donorCount,
                InfluencerCount = infCount,
                RecentInfluencerCount = recentInfCount,
                RecentDonorCount = recentDonorCount,
                DonorTimeSeries = lst,
                InfluencerTimeSeries = lst,
                ProfileByState = lst,
                DonorByOccupation = lst,
                //TopInfluencers = topInfs.Select(t => new Tuple<string, double>(t.Name, t.Count)).ToList()

            };

            return new ResInfo
            {
                Status = true,
                Data = statsDto,
                Msg = "Dashboard stats retrieved successfully",
                Desc = null
            };
        }
      
        public List<KpiInfo> ToKpiInfo(List<Tuple<string, int>> lst)
        {
            var cal = DateTime.UtcNow.AddDays(-30);
            var end = cal.Date;

            var infSeriesMap = new Dictionary<string, int>();

            // Convert the list of tuples to a dictionary
            foreach (var t in lst)
            {
                infSeriesMap[t.Item1] = t.Item2;
            }

            var dtFormat = "yyyy-MM-dd";
            var infSeries = new List<KpiInfo>();

            for (var d = end; d <= DateTime.UtcNow.Date; d = d.AddDays(1))
            {
                string dt = d.ToString(dtFormat, CultureInfo.InvariantCulture);
                int val = infSeriesMap.ContainsKey(dt) ? infSeriesMap[dt] : 0;

                infSeries.Add(new KpiInfo
                {
                    title = dt,
                    valStr = val.ToString()
                });
            }

            return infSeries;
        }


        public List<KpiInfo> ToKpiInfoByLookup(List<CountDto> profilesByState)
        {
            return profilesByState.Select(p => new KpiInfo
            {
                value = p.Count,
                title = p.State
            }).ToList();
        }



        private List<KpiInfo> ToKpiInfoValues(List<object[]> lst)
        {
            var infSeries = lst.Select(d =>
            {
                string val = d[0]?.ToString() ?? "0"; // Convert first element to string, default to "0"
                string title = d[1]?.ToString() ?? "Not Specified"; // Convert second element, default if null

                return new KpiInfo
                {
                    title = title,
                    valStr = val
                };

            }).ToList();
            return infSeries;
        }

        //private ResInfo Success(List<ProfileDto> rootProfiles)
        //{
        //    return new ResInfo
        //    {
        //        Success = true,
        //        Data = rootProfiles,
        //        Msg = "Donor-Influencer tree data retrieved successfully",
        //        Desc = null
        //    };
        //}
        public async Task<ResInfo> GetDonorInfTreeDataAsync()
        {
            List<ProfileDto> infData = _statRepo.GetDonorInfDataAsync();

            List<ProfileDto> profiles = infData.Select(t => new ProfileDto
            {
                id = GetNumValue(t.id),
                influencedById = GetNumValue(t.influencedById),
                name = t.name,
                email = t.email
            }).ToList();

            List<ProfileDto> rootProfiles = _treeUtils.Process(profiles);
            rootProfiles = rootProfiles.OrderByDescending(p => p.children.Count).Take(5).ToList();

            return Success(rootProfiles);
        }

        // Helper function to safely convert objects to long
        private long GetNumValue(object obj)
        {
            return obj != null && long.TryParse(obj.ToString(), out long value) ? value : 0;
        }

        public async Task<List<ProfileDto>> GetDonorInfTreeDataAsync(int? paramInfId, TreeUtils TreeProcessor)
        {
            ProfileDto root = null;
            if (paramInfId.HasValue && paramInfId < 0)
            {
                paramInfId = null;
            }

            if (paramInfId.HasValue)
            {
                var pm = await _profileRepository.GetByIdAsync(paramInfId.Value);
                if (pm != null)
                {
                    root = new ProfileDto
                    {
                        id = pm.id,
                        name = $"{pm.firstName} {pm.lastName}".Trim(),
                        email = pm.email
                    };
                }
            }

            List<Tuple<long, long?, string, string, string, string>> infData;
            bool isForOneInf = false;

            if (!paramInfId.HasValue)
            {
                var profileList =   _statRepo.GetDonorInfDataAsync(); // Await if async
                infData = profileList.Select(p =>
                    Tuple.Create(p.id, (long?)p.influencedById, p.name, p.email, "", "")
                ).ToList();
            }
            else
            {
                isForOneInf = true;
                var tempData = await _statRepo.GetDonorInfDataByInfIdAsync(new List<int> { paramInfId.Value });
                //infData = await _statRepo.GetDonorInfDataByInfIdAsync(new List<int> { paramInfId.Value });
                infData = tempData.Select(t =>
                Tuple.Create(t.Item1, (long?)t.Item2, t.Item3, t.Item4, t.Item5, t.Item6)
                 ).ToList();
            }


            var profiles = TupleToProfile(infData);

            if (isForOneInf)
            {
                var pIds = profiles.Select(p => (int)p.id).ToList();
                infData = await _statRepo.GetDonorInfDataByInfIdAsync(pIds);
                profiles.AddRange(TupleToProfile(infData));
            }

            var profileIds = profiles.Select(p => p.influencedById).ToList();
            var scoreList = await _statRepo.GetScoreByInfIdsAsync(profileIds);
            var scoreMap = scoreList.ToDictionary(t => t.Item1, t => t.Item1);

            profiles.ForEach(p =>
            {
                if (scoreMap.ContainsKey(p.id))
                {
                    p.name += $" ({scoreMap[p.id]})";
                }
            });

            if (root != null && scoreMap.ContainsKey(root.id))
            {
                root.name += $" ({scoreMap[root.id]})";
            }

            var rootProfiles = TreeProcessor.Process(profiles, root);
            rootProfiles = rootProfiles.OrderByDescending(w => w.children.Count).Take(5).ToList();

            return rootProfiles;
        }

        private List<ProfileDto> TupleToProfile(List<Tuple<long, long?, string, string, string, string>> infData)
        {
            return infData.Select(t => new ProfileDto
            {
                id = t.Item1,
                influencedById = t.Item2 ?? 0,
                name = $"{t.Item5} {t.Item6}".Trim(),
                email = t.Item3
            }).ToList();
        }

        public List<object[]> ProfileStatsDataByType(ProfileDto profileReq, string type)
        {
            if (profileReq == null)
            {
                profileReq = new ProfileDto();
            }

            var qb = new StringBuilder();

            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("states", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append("SELECT COUNT(a.id) AS cnt, b.state_code FROM profiles a");
                }
                else if (type.Equals("cities", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append("SELECT COUNT(a.id) AS cnt, CONCAT(b.city, ', ', b.state_code) FROM profiles a");
                }
                else
                {
                    qb.Append("SELECT COUNT(a.id) AS cnt, b.md_title FROM profiles a");
                }

                if (type.Equals("occupation", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append(" LEFT JOIN master_data b ON a.occupation_id = b.id ");
                }
                else if (type.Equals("relationship", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append(" LEFT JOIN master_data b ON a.relationship_id = b.id ");
                }
                else if (type.Equals("education", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append(" LEFT JOIN master_data b ON a.education_id = b.id ");
                }
                else if (type.Equals("states", StringComparison.OrdinalIgnoreCase) || type.Equals("cities", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append(" LEFT JOIN address b ON a.address_id = b.id ");
                }
            }

            qb.Append(" WHERE a.id IS NOT NULL ");

            if (profileReq.isDonor.HasValue && profileReq.isDonor.Value)
            {
                qb.Append(" AND a.is_donor = 1 ");
            }
            if (profileReq.isInfluencer.HasValue && profileReq.isInfluencer.Value)
            {
                qb.Append(" AND a.is_influencer = 1 ");
            }
            if (profileReq.homeCenterId > 0)
            {
                qb.Append($" AND a.home_center_id = {profileReq.homeCenterId}");
            }

            if (profileReq.relationshipId > 0)
            {
                qb.Append($" AND a.relationship_id = {profileReq.relationshipId}");
            }
            if (!string.IsNullOrEmpty(profileReq.gender))
            {
                qb.Append($" AND a.gender = '{profileReq.gender}'");
            }
            if (profileReq.ageGroup > 0)
            {
                qb.Append(" AND " + _sqlUtilService.AgeGroupQuery(profileReq.ageGroup));
            }

            if (profileReq.influencedById > 0)
            {
                qb.Append($" AND (c.id = {profileReq.influencedById} OR a.id = {profileReq.influencedById})");
            }
            if (profileReq.influencerIds != null && profileReq.influencerIds.Any())
            {
                var infIds = string.Join(",", profileReq.influencerIds);
                qb.Append($" AND c.id IN ({infIds})");
            }

            if (type.Equals("states", StringComparison.OrdinalIgnoreCase))
            {
                qb.Append(" GROUP BY b.state ");
            }
            else if (type.Equals("cities", StringComparison.OrdinalIgnoreCase))
            {
                qb.Append(" GROUP BY b.state_code, b.city ");
            }
            else
            {
                qb.Append(" GROUP BY b.md_title ");
            }

            qb.Append(" ORDER BY cnt DESC LIMIT 20 ");

            Console.WriteLine("Query::" + qb.ToString());

            return _context.profiles
                .FromSqlRaw(qb.ToString())
                .Select(p => new object[] { p.id, p.email ?? string.Empty}) // Adjust the selection as needed
                .ToList();
        }

        public ResInfo ProfileStatsData(ProfileDto profileReq)
        {
            var dbsInfo = new DashboardStatInfo();
            List<object[]> lst = new List<object[]>();

            lst = ProfileStatsDataByType(profileReq, "states");
            dbsInfo.pfsByStates = ToKpiInfoValues(lst);

            lst = ProfileStatsDataByType(profileReq, "occupation");
            dbsInfo.pfsByOccupation = ToKpiInfoValues(lst);

            lst = ProfileStatsDataByType(profileReq, "relationship");
            dbsInfo.pfsByRels = ToKpiInfoValues(lst);

            lst = ProfileStatsDataByType(profileReq, "education");
            dbsInfo.pfsByEdu = ToKpiInfoValues(lst);

            return Success(dbsInfo);
        }


        public ResInfo InfStatsData(int? hmcId)
        {
            var dbsInfo = new DashboardStatInfo();
            List<CountDto> lst ;

            if (hmcId.HasValue && hmcId < 0)
            {
                hmcId = null;
            }

            if (hmcId.HasValue)
            {
                 lst = _statRepo.GetProfilesByStateByHomeCenter(hmcId.Value);
                dbsInfo.pfsByStates = ToKpiInfoByLookup(lst);

                lst = _statRepo.GetInfuencersByOccupationByHomeCenter(hmcId.Value);
                dbsInfo.pfsByOccupation = ToKpiInfoByLookup(lst);

                lst = _statRepo.GetInfluencersByRelByHomeCenter(hmcId.Value);
                dbsInfo.pfsByRels = ToKpiInfoByLookup(lst);

                lst = _statRepo.GetInfluencersByEduByHomeCenter(hmcId.Value);
                dbsInfo.pfsByEdu = ToKpiInfoByLookup(lst);
            }
            else
            {
                lst = _statRepo.GetProfilesByState();
                dbsInfo.pfsByStates = ToKpiInfoByLookup(lst);

                lst = _statRepo.GetInfuencersByOccupation();
                dbsInfo.pfsByOccupation = ToKpiInfoByLookup(lst);

                lst = _statRepo.GetInfuencersByRel();
                dbsInfo.pfsByRels = ToKpiInfoByLookup(lst);

                lst = _statRepo.GetInfuencersByEdu();
                dbsInfo.pfsByEdu = ToKpiInfoByLookup(lst);
            }

            return Success(dbsInfo);
        }



        public ResInfo InfDetailsData(ProfileDto pDto)
        {
            if (pDto == null)
            {
                pDto = new ProfileDto();
            }

            ResInfo res = _profileService.SearchProfiles(pDto);
            return res;
        }

        public async Task<ResInfo> InfluencerTreeData(ProfileDto profileReq)
        {
            if (profileReq == null) return Success(new List<ProfileDto>());

            List<ProfileDto> profiles = getProfileList(profileReq);

            if (profileReq.influencedById > 0)
            {
                List<long> pIds = profiles
                    .Where(p => p.id != profileReq.influencedById)
                    .Select(p => p.id)
                    .ToList();

                profiles.AddRange(getProfileList(new ProfileDto { influencerIds = pIds }));
            }

            List<long> profileIds = profiles
                .Where(p => p.influencedById > 0)
                .Select(p => p.influencedById)
                .ToList();

            var scoreMap = (await _statRepo.GetScoreByInfIdsAsync(profileIds))
                .ToDictionary(t => t.Item1, t => (int)t.Item2);

            profiles.ForEach(p =>
            {
                if (scoreMap.ContainsKey(p.id))
                {
                    p.name += $" ({scoreMap[p.id]})";
                }
            });

            var rootProfiles = _treeUtils.Process(profiles, null)
                .OrderByDescending(w => w.children.Count)
                .Take(5)
                .ToList();

            return Success(rootProfiles);
        }

        private List<ProfileDto> getProfileList(ProfileDto profileReq)
        {
            if (profileReq == null)
            {
                profileReq = new ProfileDto();
            }

            String profileQuery = "SELECT a.id, c.id, a.email, c.email, a.first_name, a.last_name, c.first_name, c.last_name " +
                                  "FROM profiles AS a " +
                                  "LEFT JOIN donar_influencer_map AS b ON b.profile_id = a.id " +
                                  "LEFT JOIN profiles AS c ON b.influenced_by = c.id " +
                                  "WHERE a.id IS NOT NULL ";

            StringBuilder qb = new StringBuilder(profileQuery);
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (Boolean.TrueString.Equals(profileReq.isDonor))
            {
                qb.Append(" AND a.is_donor = true ");
            }
            if (Boolean.TrueString.Equals(profileReq.isInfluencer))
            {
                qb.Append(" AND a.is_influencer = true ");
            }
            if (profileReq.homeCenterId > 0)
            {
                qb.Append(" AND a.home_center_id = @homeCenterId ");
                parameters.Add("homeCenterId", profileReq.homeCenterId);
            }
            if (profileReq.relationshipId > 0)
            {
                qb.Append(" AND a.relationship_id = @relationshipId ");
                parameters.Add("relationshipId", profileReq.relationshipId);
            }
            if (!string.IsNullOrEmpty(profileReq.gender))
            {
                qb.Append(" AND a.gender = @gender ");
                parameters.Add("gender", profileReq.gender);
            }
            if (profileReq.ageGroup > 0)
            {
                qb.Append(" AND " + _sqlUtilService.AgeGroupQuery(profileReq.ageGroup));
            }
            if (profileReq.influencedById > 0)
            {
                qb.Append(" AND (c.id = @influencedById OR a.id = @influencedById) ");
                parameters.Add("influencedById", profileReq.influencedById);
            }
            if (profileReq.influencerIds != null && profileReq.influencerIds.Any())
            {
                qb.Append(" AND c.id IN (@influencerIds) ");
                parameters.Add("influencerIds", profileReq.influencerIds);
            }

            var query = _context.profiles.FromSqlRaw(qb.ToString());
            foreach (var parameter in parameters)
            {
                query = query.AsQueryable().Where(p => EF.Property<object>(p, parameter.Key) == parameter.Value);
            }
            List<ProfileModel> list = query.ToList();
            return ObjectsToProfile(list.Select(p => new object[] { p.id, p.email, p.firstName, p.lastName }).ToList());
        }

        private List<ProfileDto> ObjectsToProfile(List<object[]> infData)
        {
            List<ProfileDto> profiles = infData
                .Select(t =>
                {
                    long? id = NullUtils.GetNumValue(t[0]);
                    long? infId = NullUtils.GetNumValue(t[1]);
                    string firstName = Convert.ToString(t[4]);
                    string lastName = Convert.ToString(t[5]);

                    return new ProfileDto
                    {
                        name = NameUtils.Appender(" ", firstName, lastName),
                        email = Convert.ToString(t[2]),
                        id = (long)id,
                        influencedById = (long)infId
                    };
                }).ToList();

            return profiles;
            
        }

        public ResInfo DonorStatsHmData(ProfileDto pDto)
        {
            // Initialize DashboardStatInfo
            var dbsInfo = new DashboardStatInfo();

            // Fetch statistics data by type "cities"
            List<object[]> lst = ProfileStatsDataByType(pDto, "cities");

            // Convert data to KPI values and assign to `PfsByCities`
            dbsInfo.pfsByCities = ToKpiInfoValues(lst);

            // Return success response
            return Success(dbsInfo);
        }

    }
}
