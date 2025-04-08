
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
using PlasmaDonorAPI.Dto;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Data;

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
        ) : base(logger, httpContextAccessor)
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

            int donorCount = await _statRepo.GetDonorCountAsync();
            int infCount = await _statRepo.GetInfluencerCountAsync();
            DateTime date30DaysAgo = DateTime.Now.AddDays(-30);
            int recentInfCount = await _statRepo.GetRecentInfluencerCountAsync(date30DaysAgo);
            int recentDonorCount = await _statRepo.GetRecentDonorCountAsync(date30DaysAgo);


            var dbsInfo = new DashboardStatInfo
            {
                topCards = new List<KpiInfo>
                {
                new KpiInfo { title = "Donors",value=donorCount, valStr = donorCount.ToString(), bgColor = "success", icon = "bi bi-wallet" },
                new KpiInfo { title = "Influencers",value=infCount, valStr = infCount.ToString(), bgColor = "danger", icon = "bi bi-brightness-high" },
                new KpiInfo { title = "New Donors",value=recentDonorCount, valStr = recentDonorCount.ToString(), bgColor = "warning", icon = "bi bi-book-half" },
                new KpiInfo { title = "New Influencers",value=recentInfCount, valStr = recentInfCount.ToString(), bgColor = "info", icon = "bi bi-box-fill" }
                }
            };


            dbsInfo.donorSeries = ToKpiInfo(_statRepo.GetDonorTimeSeries(DateTime.Now).ToList());
            dbsInfo.infSeries = ToKpiInfo(_statRepo.GetInfTimeSeries(DateTime.Now)).ToList();
            dbsInfo.pfsByStates = ToKpiInfoByLookup(_statRepo.GetProfilesByState().ToList());
            dbsInfo.pfsByOccupation = ToKpiInfoByLookup(_statRepo.GetDonorsByOccupation().ToList());

            var topInfs = _statRepo.GetTopInfluencers();
            dbsInfo.topInfluencers = topInfs.Select(t =>
            {
                string firstName = t.FirstName?.ToString() ?? "";
                string lastName = t.LastName?.ToString() ?? "";
                string icnTxt = lastName.Length >= 2 ? lastName.Substring(0, 2) : lastName;

                if (!string.IsNullOrEmpty(firstName))
                {
                    icnTxt = firstName.Substring(0, 1) + (lastName.Length > 0 ? lastName.Substring(0, 1) : "");
                }

                return new TopInfluencerDto
                {
                    name = NameUtils.Appender(" ", firstName, lastName),
                    email = t.Email?.ToString() ?? "",
                    location = NameUtils.Appender(", ",
                        t.AddressLine?.ToString() ?? "",
                        t.City?.ToString() ?? "",
                        t.State?.ToString() ?? ""),
                    count = (double)NameUtils.DoubleVal(t.InfScore),
                    icn = icnTxt.ToUpper(),
                    cssCls = "success"
                };

            }).ToList();
            _logger.LogInformation($"Dashboard Data: {JsonConvert.SerializeObject(dbsInfo)}");

            //return Success(dbsInfo);
            return new ResInfo
            {
                Status = true,
                Data = dbsInfo,
                Msg = "success"
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
                    value= val,
                    valStr = val.ToString()
                });
            }

            return infSeries;
        }


        public List<KpiInfo> ToKpiInfoByLookup(List<Tuple<string, int>> lst)
        {
            return lst.Select(d =>
            {
                Int64 value = d.Item2;
                string val = d.Item2.ToString();
                string title = d.Item1;

                if (string.IsNullOrEmpty(title))
                {
                    title = "Not Specified";
                }

                return new KpiInfo
                {
                    value = value,
                    valStr = val,
                    title = title
                };
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

        public List<KpiInfo> ProfileStatsDataByType(ProfileDto profileReq, string type)
        {
            try
            {

                if (profileReq == null)
                {
                    profileReq = new ProfileDto();
                }

                var qb = new StringBuilder();
                var parameters = new List<object>();

                qb.Append("SELECT IFNULL(title,'Not Mentioned') AS title, CONVERT(IFNULL(value,0),CHAR) AS valStr, NULL AS bgColor, NULL AS icon, IFNULL(value,0) AS value, NULL AS cont FROM (");

                // Base query setup based on type
                if (!string.IsNullOrEmpty(type))
                {
                    if (type.Equals("states", StringComparison.OrdinalIgnoreCase))
                    {
                        qb.Append("SELECT COUNT(a.id) AS value, b.state_code AS title FROM profiles a");
                    }
                    else if (type.Equals("cities", StringComparison.OrdinalIgnoreCase))
                    {
                        qb.Append("SELECT COUNT(a.id) AS value, CONCAT(b.city, ', ', b.state_code) AS title FROM profiles a");
                    }
                    else
                    {
                        qb.Append("SELECT COUNT(a.id) AS value, b.md_title AS title FROM profiles a");
                    }

                    // Handle joins
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

                // Add filters to query and corresponding parameters
                if (profileReq.isDonor.HasValue && profileReq.isDonor.Value)
                {
                    qb.Append($" AND a.is_donor = {profileReq.isDonor}");
                    parameters.Add(1);
                }
                if (profileReq.isInfluencer.HasValue && profileReq.isInfluencer.Value)
                {
                    qb.Append($" AND a.is_influencer = {profileReq.isInfluencer}");
                    parameters.Add(1);
                }
                if (profileReq.homeCenterId > 0)
                {
                    qb.Append($" AND a.home_center_id = {profileReq.homeCenterId}");
                    parameters.Add(profileReq.homeCenterId);
                    //qb.Append(" AND a.home_center_id = @homeCenterId");
                    //parameters.Add(new MySqlParameter("@homeCenterId", profileReq.homeCenterId));

                }

                if (profileReq.relationshipId > 0)
                {
                    qb.Append($" AND a.relationship_id = {profileReq.relationshipId}");
                    parameters.Add(profileReq.relationshipId);
                }
                if (!string.IsNullOrEmpty(profileReq.gender))
                {
                    //qb.Append($" AND a.gender = {profileReq.gender}");
                    //parameters.Add(profileReq.gender);
                    //qb.Append(" AND a.gender = '{profileReq.gender}'");

                    qb.Append("AND gender = @Gender");
                    parameters.Add(new MySqlParameter("@Gender", profileReq.gender));


                }
                if (profileReq.ageGroup > 0)
                {
                    qb.Append(" AND " + _sqlUtilService.AgeGroupQuery((int)profileReq.ageGroup));
                }

                if (profileReq.influencedById > 0)
                {
                    qb.Append($" AND (c.id = {profileReq.influencedById} OR a.id = {profileReq.influencedById})");
                    parameters.Add(profileReq.influencedById);
                }
                if (profileReq.influencerIds != null && profileReq.influencerIds.Any())
                {
                    var infIds = string.Join(",", profileReq.influencerIds);
                    qb.Append($" AND c.id IN ({infIds})");
                }

                // Add GROUP BY and ORDER BY clauses
                if (type.Equals("states", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append(" GROUP BY b.state_code ");
                }
                else if (type.Equals("cities", StringComparison.OrdinalIgnoreCase))
                {
                    qb.Append(" GROUP BY b.state_code, b.city ");
                }
                else
                {
                    qb.Append(" GROUP BY b.md_title ");
                }

                qb.Append(") AS t");

                qb.Append(" ORDER BY value DESC LIMIT 20 ");

                Console.WriteLine("Query::" + qb.ToString());

                //Execute the query with parameters
                return _context.Database
                    .SqlQueryRaw<KpiInfo>(qb.ToString())
                    .ToList();
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public ResInfo ProfileStatsData(ProfileDto profileReq)
        {
            var dbsInfo = new DashboardStatInfo();
            
            dbsInfo.pfsByStates = ProfileStatsDataByType(profileReq, "states");

            dbsInfo.pfsByOccupation = ProfileStatsDataByType(profileReq, "occupation");

            dbsInfo.pfsByRels = ProfileStatsDataByType(profileReq, "relationship");

            dbsInfo.pfsByEdu = ProfileStatsDataByType(profileReq, "education");

            return Success(dbsInfo);
        }


        public ResInfo InfStatsData(int? hmcId)
        {
            var dbsInfo = new DashboardStatInfo();
            List<Tuple<string, int>> lst;

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

            if (profileReq?.influencedById != null)
            {
                List<long> pIds = profiles
                    .Where(p => p.id != null && profileReq.influencedById != null && p.id != profileReq.influencedById)
                    .Select(p => (long)p.id)
                    .ToList();

                getProfileList(new ProfileDto { influencerIds = pIds });
            }

            List<long?> profileIds = profiles
                .Where(p => p.id != null)
                .Select(p => p.influencedById)
                .ToList();

            var scoreMap = (await _statRepo.GetScoreByInfIdsAsync(profileIds))
                .ToDictionary(t => t.Item1, t => (double)t.Item2);

            profiles.ForEach(p =>
            {
                long profileId = (long)p.id;
                //double score = (double)p.infScore;
                if (scoreMap.TryGetValue(profileId, out double score))
                {
                    p.name = p.name ?? $"{p.firstName} {p.lastName}";
                    p.name += $" ({score})";
                    p.infScore = score;
                    //p.name += $" ({scoreMap[(long)p.id]})";
                }
            });

            var rootProfiles = _treeUtils.Process(profiles, null)
                 .Select(p => new ProfileDto
                 {
                     id = p.id,
                     name = p.name,
                     email = p.email,
                     firstName = p.firstName,
                     lastName = p.lastName,
                     influencedById = p.influencedById,
                     interests = p.interests,
                     infScore = p.infScore,
                     children = p.children ?? new List<ProfileDto>(),
                     isExpanded = (p.infScore > 0 && (p.children?.Count ?? 0) > 0)
                     
                 })
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

            string profileQuery = @"
        SELECT a.id AS donorId, c.id AS influencerId, 
               a.email AS donorEmail, c.email AS influencerEmail, 
               a.first_name AS donorFirstName, a.last_name AS donorLastName, 
               c.first_name AS influencerFirstName, c.last_name AS influencerLastName
        FROM profiles AS a
        LEFT JOIN donar_influencer_map AS b ON b.profile_id = a.id
        LEFT JOIN profiles AS c ON b.influenced_by = c.id
        WHERE a.id IS NOT NULL ";

            List<object> sqlParams = new List<object>();
            List<string> conditions = new List<string>();

            if (profileReq.isDonor == true)
            {
                conditions.Add("a.is_donor = @isDonor");
                sqlParams.Add(new SqlParameter("@isDonor", true));
            }
            if (profileReq.isInfluencer == true)
            {
                conditions.Add("a.is_influencer = @isInfluencer");
                sqlParams.Add(new SqlParameter("@isInfluencer", true));
            }
            if (profileReq.homeCenterId > 0)
            {
                conditions.Add("a.home_center_id = @homeCenterId");
                sqlParams.Add(new SqlParameter("@homeCenterId", profileReq.homeCenterId));
            }
            if (profileReq.relationshipId > 0)
            {
                conditions.Add("a.relationship_id = @relationshipId");
                sqlParams.Add(new SqlParameter("@relationshipId", profileReq.relationshipId));
            }
            if (!string.IsNullOrEmpty(profileReq.gender))
            {
                conditions.Add("a.gender = @gender");
                sqlParams.Add(new SqlParameter("@gender", profileReq.gender));
            }
            if (profileReq.ageGroup > 0)
            {
                conditions.Add(_sqlUtilService.AgeGroupQuery((int)profileReq.ageGroup));
            }
            if (profileReq.influencedById > 0)
            {
                conditions.Add("(c.id = @influencedById OR a.id = @influencedById)");
                sqlParams.Add(new SqlParameter("@influencedById", profileReq.influencedById));
            }
            if (profileReq.influencerIds != null && profileReq.influencerIds.Any())
            {
                string influencerIdParams = string.Join(",", profileReq.influencerIds);
                conditions.Add($"c.id IN ({influencerIdParams})");
            }

            if (conditions.Any())
            {
                profileQuery += " AND " + string.Join(" AND ", conditions);
            }

            //var query = _context.profiles.FromSqlRaw(profileQuery, sqlParams.ToArray()).ToList();

            //return query.Select(p => new ProfileDto
            //{
            //    id = p.donorId,
            //    email = p.donorEmail,
            //    firstName = p.donorFirstName,
            //    lastName = p.donorLastName,
            //    influencedById = p.influencerId,
            //    InfluencerEmail = p.influencerEmail,
            //    InfluencerFirstName = p.influencerFirstName,
            //    InfluencerLastName = p.influencerLastName
            //}).ToList();

            List<ProfileDto> profiles = new List<ProfileDto>();

            var conn = _context.Database.GetDbConnection();
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = profileQuery;
                    cmd.CommandType = CommandType.Text;

                    foreach (var param in sqlParams)
                    {
                        cmd.Parameters.Add(param);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            profiles.Add(new ProfileDto
                            {
                                id = reader["donorId"] != DBNull.Value ? Convert.ToInt32(reader["donorId"]) : 0,
                                email = reader["donorEmail"]?.ToString(),
                                firstName = reader["donorFirstName"]?.ToString(),
                                lastName = reader["donorLastName"]?.ToString(),
                                influencedById = reader["influencerId"] != DBNull.Value ? Convert.ToInt32(reader["influencerId"]) : 0,
                                name = NameUtils.Appender(" ", reader["donorFirstName"].ToString(), reader["donorLastName"].ToString())

                            });
                        }
                    }
                }
            }

            return profiles;

        }


        private List<ProfileDto> ObjectsToProfile(List<object[]> infData)
        {
            //List<ProfileDto> profiles = infData
            //    .Select(t =>
            return infData.Select(t => new ProfileDto
                {
                     id = NullUtils.GetNumValue(t[0]) ?? 0,
                     influencedById = NullUtils.GetNumValue(t[1]) ?? 0,
                     email = Convert.ToString(t[2]),
                     firstName = Convert.ToString(t[4]),
                     lastName= Convert.ToString(t[5]),
                     name = NameUtils.Appender(" ", Convert.ToString(t[4]), Convert.ToString(t[5]))

                    //return new ProfileDto
                    //{
                    //    name = NameUtils.Appender(" ", firstName, lastName),
                    //    email = Convert.ToString(t[2]),
                    //    id = (long)id,
                    //    influencedById = (long)infId
                    //};
                }).ToList();

            //return profiles;

        }

        public ResInfo DonorStatsHmData(ProfileDto pDto)
        {
            // Initialize DashboardStatInfo
            var dbsInfo = new DashboardStatInfo();

            // Fetch statistics data by type "cities"
            //List<object[]> lst = ProfileStatsDataByType(pDto, "cities");

            // Convert data to KPI values and assign to `PfsByCities`
            dbsInfo.pfsByCities = ProfileStatsDataByType(pDto, "cities");          

            // Return success response
            return Success(dbsInfo);
        }

    }
}
