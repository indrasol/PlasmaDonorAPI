using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.utils;
namespace NewPlasmaDonorsAPI.Services
{
    public class MdService : BaseService
    {
        private readonly AppDbContext _context;
        private readonly MdRepository _mdRepository;
        private readonly ILogger<MdService> _logger;


        public MdService(AppDbContext context, MdRepository mdRepository, ILogger<MdService> logger, IHttpContextAccessor httpContextAccessor) : base(logger,httpContextAccessor)
        {
            _context = context;
            _mdRepository = mdRepository;
            _logger = logger;
        }

        public interface IMasterDataService
        {
            Task<ResInfo> GetMasterDataByTypesAsync(List<string> types);
        }

        // Save master data (currently a placeholder)
        public ResInfo SaveMasterData()
        {
            // Placeholder for actual logic
            return null!;
        }

        // Get master data for a specific type
        public ResInfo GetMasterData(string type)
        {
            _logger.LogInformation("Getting master data for type: {type}", type);
            // Placeholder logic to fetch master data for the given type
            return null!;
        }

        // Get master data for profile (occupation, race, language, relationship)
        public ResInfo GetMasterDataForProfile()
        {
            _logger.LogInformation("Getting master data for profile");

            var types = new List<string> { "occupation", "education", "race", "hobbies", "language", "relationship","interests"};
            var mdMap = new Dictionary<string, List<MdInfo>>();

            foreach (var type in types)
            {
                var mdList = _context.MasterData!.Where(m => m.mdType == type).ToList();
                mdMap[type] = mdList.Select(m => new MdInfo
                {
                    Id = m.id,
                    Name = m.mdTitle!,
                    Status = m.status!
                }).ToList();
            }

            return Success(mdMap);
        }

        // Get a specific MasterData entry by ID
        public MasterData? GetMdEntry(long id)
        {
            if (NullUtils.IsValid(id))
            {
                return _context.MasterData?.Find(id);
            }
            return null;
        }

        public Task<ResInfo> GetMasterDataByTypesAsync(List<string> types)
        {
            _logger.LogInformation($"Getting master data for types: {string.Join(", ", types)}");

            var mdMap = new Dictionary<string, List<MdInfo>>();

            foreach (var type in types)
            {
                var mdList = _mdRepository.FindAllByMdType(type);
                mdMap[type] = mdList.Select(m => new MdInfo
                {
                    Id = m.id,
                    Name = m.mdTitle ?? string.Empty, // Fix for CS8601
                    Status = m.status ?? string.Empty // Fix for CS8601
                }).ToList();
            }

            return Task.FromResult(Success(mdMap));
        }

    }
}

