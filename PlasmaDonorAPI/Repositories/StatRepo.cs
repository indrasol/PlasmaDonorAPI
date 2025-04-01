using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using MySqlConnector;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Dto.Dashboard;
using Microsoft.Data.SqlClient;
using StructureMap;
using PlasmaDonorAPI.Dto;
public class StatRepo : IStatRepo
{
    private readonly AppDbContext _context;

    public StatRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetDonorCountAsync()
    {
        if (_context.profiles == null)
        {
            throw new InvalidOperationException("Profiles DbSet is null.");
        }

        return await _context.profiles
            .Where(p => p.isDonor == true)
            .CountAsync();
    }

    public async Task<int> GetInfluencerCountAsync()
    {
        if (_context.profiles == null)
        {
            throw new InvalidOperationException("Profiles DbSet is null.");
        }

        return await _context.profiles
            .Where(p => p.isInfluencer == true)
            .CountAsync();
    }
    public async Task<int> GetRecentInfluencerCountAsync(DateTime date)
    {
        if (_context.profiles == null)
        {
            throw new InvalidOperationException("Profiles DbSet is null.");
        }

        return await _context.profiles
            .Where(p => p.isInfluencer == true && p.createdOn >= date)
            .CountAsync();
    }
    public async Task<int> GetRecentDonorCountAsync(DateTime date)
    {
        if (_context.profiles == null)
        {
            throw new InvalidOperationException("Profiles DbSet is null.");
        }

        return await _context.profiles
            .Where(p => p.isDonor == true && p.createdOn >= date)
            .CountAsync();
    }

    //public async Task<int> GetRecentDonorCountAsync(DateTime date)
    //{
    //    // Raw SQL query to get the count of donors created after the specified date
    //    var query = "SELECT COUNT(id) FROM profiles WHERE is_donor = true AND created_on >= @date";

    //    // Execute the query asynchronously and return the result
    //    var result = await _context.Database.ExecuteSqlRawAsync(query, new MySqlParameter("@date", date));

    //    return result;
    //}
    public List<Tuple<string, int>> GetDonorTimeSeries(DateTime date)
    {
        if (_context.DonorTimeSeriesResults == null)
        {
            throw new InvalidOperationException("DonorTimeSeriesResults DbSet is null.");
        }

        var query = _context.DonorTimeSeriesResults
            .FromSqlRaw(
                "SELECT count(id) as Cont, DATE_FORMAT(created_on, '%Y-%m-%d') as Title " +
                "FROM profiles WHERE is_donor = 1 AND created_on >= {0} " +
                "GROUP BY DATE_FORMAT(created_on, '%Y-%m-%d') " +
                "ORDER BY Title ASC",
                date // Parameter passed using index-based formatting
            )
            .ToList();

        return query.Select(x => new Tuple<string, int>(x.Title, x.Cont)).ToList();
    }



    //public List<Tuple<string, int>> GetProfilesByState()
    //{
    //    string query = @"
    //    SELECT COUNT(a.id) AS Count, adr.state AS State   
    //    FROM profiles a 
    //    LEFT JOIN address adr ON a.address_id = adr.id 
    //    WHERE a.is_influencer = 1  
    //    GROUP BY adr.state    
    //    ORDER BY Count DESC;";
    //    // return _context.Database.SqlQueryRaw<CountDto>(query).ToList();
    //    var result = _context.Database.SqlQueryRaw<CountDto>(query).ToList();

    //    // Convert List<CountDto> to List<Tuple<string, int>>
    //    return result.Select(r => Tuple.Create(r.State, r.Count)).ToList();
    //}


    public List<Tuple<string, int>> GetInfTimeSeries(DateTime date)
    {
        if (_context.InfluencerTimeSeriesResults == null)
        {
            throw new InvalidOperationException("InfluencerTimeSeriesResults DbSet is null.");
        }

        var query = _context.InfluencerTimeSeriesResults
            .FromSqlRaw(
                "SELECT count(id) as Cont, DATE_FORMAT(created_on, '%Y-%m-%d') as Title " +
                "FROM profiles WHERE is_influencer = 1 AND created_on >= {0} " +
                "GROUP BY DATE_FORMAT(created_on, '%Y-%m-%d') " +
                "ORDER BY Title ASC",
                date
            )
            .ToList();

        return query.Select(x => new Tuple<string, int>(x.Title ?? string.Empty, x.Cont)).ToList();
    }

    public List<Tuple<string, int>> GetProfilesByState()
    {
        if (_context.ProfileByStateResults == null)
        {
            throw new InvalidOperationException("ProfileByStateResults DbSet is null.");
        }

        var query = _context.ProfileByStateResults
            .FromSqlRaw(
                "SELECT COUNT(a.id) AS Cnt, COALESCE(adr.state, '') AS Str " +
                "FROM profiles a " +
                "LEFT JOIN address adr ON a.address_id = adr.id " +
                "WHERE a.is_influencer = 1 " +  // TRUE -> 1 in SQL
                "GROUP BY adr.state " +
                "ORDER BY Cnt DESC"
            )
            .ToList();

        return query.Select(x => new Tuple<string, int>(x.Str, x.Cnt)).ToList();
    }

    public List<Tuple<string,int>> GetProfilesByStateByHomeCenter(int hmcId)
    {
        string query = "SELECT COUNT(a.id) AS cnt, adr.state AS str " +
                       "FROM profiles a " +
                       "LEFT JOIN address adr ON a.address_id = adr.id " +
                       "WHERE a.is_influencer = 1 " +
                       "AND a.home_center_id = @hmcId " +
                       "GROUP BY adr.state " +
                       "ORDER BY cnt DESC;";


        var result = _context.DonorByOccupationResults
         .FromSqlRaw(query, new SqlParameter("@hmcId", hmcId))
         .ToList();

        return result.Select(r => new Tuple<string, int>(
            r.MdTitle ?? "Not Specified",  // Extract md_title
            r.Cnt                          // Extract count
        )).ToList();
    }

    public List<Tuple<string, int>> GetDonorsByOccupation()
    {
        if (_context.DonorByOccupationResults == null)
        {
            throw new InvalidOperationException("DonorByOccupationResults DbSet is null.");
        }

        var query = _context.DonorByOccupationResults
            .FromSqlRaw(
                "SELECT COUNT(a.id) AS Cnt, b.md_title AS MdTitle " +
                "FROM profiles a " +
                "LEFT JOIN master_data b ON a.occupation_id = b.id " +
                "WHERE a.is_donor = 1 " +  // TRUE -> 1 in SQL
                "GROUP BY b.md_title " +
                "ORDER BY Cnt DESC " +
                "LIMIT 20"
            )
                .Select(x => new DonorByOccupationResults { MdTitle = x.MdTitle, Cnt = x.Cnt }) // Map to DTO
                .ToList();

        return query.Select(x => new Tuple<string, int>(x.MdTitle ?? string.Empty, x.Cnt)).ToList();
    }

    public List<Tuple<string,int>> GetInfuencersByOccupationByHomeCenter(int hmcId)
    {
        string query = @"SELECT COUNT(a.id) AS cnt, b.md_title 
                     FROM profiles a  
                     LEFT JOIN master_data b ON a.occupation_id = b.id  
                     WHERE a.is_influencer = 1 AND a.home_center_id = @hmcId
                     GROUP BY b.md_title  
                     ORDER BY cnt DESC   
                     LIMIT 8;";

        var result = _context.DonorByOccupationResults
         .FromSqlRaw(query, new SqlParameter("@hmcId", hmcId))
         .ToList();

        return result.Select(r => new Tuple<string, int>(
            r.MdTitle ?? "Not Specified",  // Extract md_title
            r.Cnt                          // Extract count
        )).ToList();
    }

    public List<Tuple<string, int>> GetInfluencersByRelByHomeCenter(int hmcId)
    {
        string query = @"
        SELECT COUNT(a.id) AS cnt, b.md_title 
        FROM profiles a  
        LEFT JOIN master_data b ON a.relationship_id = b.id  
        WHERE a.is_influencer = 1 AND a.relationship_id = @hmcId
        GROUP BY b.md_title  
        ORDER BY cnt DESC   
        LIMIT 20;";

        var result = _context.DonorByOccupationResults
          .FromSqlRaw(query, new SqlParameter("@hmcId", hmcId))
          .ToList();

        return result.Select(r => new Tuple<string, int>(
            r.MdTitle ?? "Not Specified",  // Extract md_title
            r.Cnt                          // Extract count
        )).ToList();

    }

    public List<Tuple<string,int>> GetInfluencersByEduByHomeCenter(int hmcId)
    {
        string query = @"
        SELECT COUNT(a.id) AS cnt, b.md_title     
        FROM profiles a  
        LEFT JOIN master_data b ON a.education_id = b.id  
        WHERE a.is_influencer = 1 AND a.home_center_id = @hmcId 
        GROUP BY b.md_title  
        ORDER BY cnt DESC   
        LIMIT 20;";

        var result = _context.DonorByOccupationResults
          .FromSqlRaw(query, new SqlParameter("@hmcId", hmcId))
          .ToList();

        return result.Select(r => new Tuple<string, int>(
            r.MdTitle ?? "Not Specified",  // Extract md_title
            r.Cnt                          // Extract count
        )).ToList();
    }

    public List<Tuple<string,int>> GetInfuencersByOccupation()
    {

        var query = _context.ProfileByStateResults
         .FromSqlRaw(
        "SELECT COUNT(a.id) AS cnt, b.md_title" +
        "FROM profiles a" +
        "LEFT JOIN master_data b ON a.occupation_id = b.id" +
        "WHERE a.is_influencer = 1" +
        "GROUP BY b.md_title" +
        "ORDER BY cnt DESC" +
        "LIMIT 20"
        )
         .ToList();

        return query.Select(x => new Tuple<string, int>(x.Str, x.Cnt)).ToList();
    }

    public List<Tuple<string, int>> GetInfuencersByRel()
    {
        var query = _context.ProfileByStateResults
         .FromSqlRaw(
        "SELECT COUNT(a.id) AS cnt, b.md_title" +
        "FROM profiles a " +
        "LEFT JOIN master_data b ON a.relationship_id = b.id " +
        "WHERE a.is_influencer = 1 " +
        "GROUP BY b.md_title" +
        "ORDER BY cnt DESC" +
        "LIMIT 20"
        )
         .ToList();

        return query.Select(x => new Tuple<string, int>(x.Str, x.Cnt)).ToList();
    }

    public List<Tuple<string, int>> GetInfuencersByEdu()
    {
        var query = _context.ProfileByStateResults
         .FromSqlRaw(
        "SELECT COUNT(a.id) AS cnt, b.md_title" +
        "FROM profiles a" +
        "LEFT JOIN master_data b ON a.education_id = b.id" +
        "WHERE a.is_influencer = 1" +
        "GROUP BY b.md_title" +
        "ORDER BY cnt DESC" +
        "LIMIT 20"
        ).ToList();

        return query.Select(x => new Tuple<string, int>(x.Str, x.Cnt)).ToList();
    }

    public List<TopInfluencerInfo> GetTopInfluencers()
    {
        string query = @"
            SELECT 
                b.first_name AS FirstName, 
                b.last_name AS LastName, 
                b.email AS Email,  
                e.address_line AS AddressLine, 
                e.city AS City, 
                e.state AS State, 
                e.country AS Country,  
                SUM(d.md_score) AS InfScore 
            FROM donar_influencer_map a  
            LEFT JOIN profiles b ON b.id = a.influenced_by  
            LEFT JOIN profiles c ON c.id = a.profile_id  
            LEFT JOIN master_data d ON c.relationship_id = d.id  
            LEFT JOIN address e ON b.address_id = e.id  
            GROUP BY a.influenced_by  
            ORDER BY InfScore DESC  
            LIMIT 20";

        return _context.TopInfluencersinfo
            .FromSqlRaw(query)
            .ToList();
    }
    
    public List<ProfileDto> GetDonorInfDataAsync()
    {
        string sql = @"
            SELECT a.id AS DonorId, 
                   c.id AS InfluencerId, 
                   a.email AS DonorEmail, 
                   c.email AS InfluencerEmail, 
                   a.first_name AS DonorFirstName, 
                   a.last_name AS DonorLastName, 
                   c.first_name AS InfluencerFirstName, 
                   c.last_name AS InfluencerLastName
            FROM profiles AS a 
            LEFT JOIN donar_influencer_map AS b ON b.profile_id = a.id  
            LEFT JOIN profiles AS c ON b.influenced_by = c.id";

        return _context.Set<ProfileDto>().FromSqlRaw(sql).ToList();
    }

    public async Task<List<Tuple<long, long?, string, string, string, string>>> GetDonorInfDataByInfIdAsync(List<int> infIds)
    {
        if (infIds == null || !infIds.Any())
        {
            return new List<Tuple<long, long?, string, string, string, string>>();
        }
        if (_context.profiles == null)
        {
            throw new InvalidOperationException("Profiles DbSet is null.");
        }

        string sqlQuery = @"
            SELECT a.id AS DonorId, c.id AS InfluencerId, 
                   a.email AS DonorEmail, c.email AS InfluencerEmail,
                   a.first_name AS DonorFirstName, a.last_name AS DonorLastName
            FROM profiles AS a 
            LEFT JOIN donar_influencer_map AS b ON b.profile_id = a.id  
            LEFT JOIN profiles AS c ON b.influenced_by = c.id  
            WHERE c.id IN ({0})";

        var result = await _context.Set<DonorInfluencerDto>()
            .FromSqlRaw(sqlQuery, infIds.ToArray()) // Pass array of IDs as parameter
            .ToListAsync();

        return result.Select(x => new Tuple<long, long?, string, string, string, string>(
            x.DonorId, x.InfId, x.DonorEmail, x.InfluencerEmail, x.DonorFirstName, x.DonorLastName)).ToList();
    }
    public async Task<List<Tuple<long,double>>> GetScoreByInfIdsAsync(List<long> infIds)
    {
        if (infIds == null || !infIds.Any())
            return new List<Tuple<long, double>>();

        return await _context.profiles
            .Where(s => infIds.Contains(s.id))
            .Select(s => new Tuple<long, double>(s.id, s.infScore))
            .ToListAsync();
    }
}


