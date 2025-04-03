using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Services.excel;
using NewPlasmaDonorsAPI.Services.Validator;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Services;
using Serilog;

namespace NewPlasmaDonorsAPI.Services
{
    public class ProfileImportService : BaseService
    {
        private readonly ExcelProcessor _excelProcessor;
        private readonly ProfileService _profileService;
        private readonly ProfileValidator _profileValidator;
        private readonly ILogger<ProfileImportService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProfileImportService(ExcelProcessor excelProcessor, ProfileService profileService,
            ProfileValidator profileValidator, ILogger<ProfileImportService> logger, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
        {
            _excelProcessor = excelProcessor;
            _profileService = profileService;
            _profileValidator = profileValidator;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResInfo> ImportProfiles(IFormFile xlFile)
        {
            if (xlFile == null || xlFile.Length == 0)
            {
                return new ResInfo { Status = false, Msg = "No file uploaded.", Data = new List<ProfileDto>() };
            }

            // Process the Excel file and convert to ProfileDto list
            var profiles = await Task.Run(() => _excelProcessor.ExcelToProfiles(xlFile));
            Log.Information(":: Profiles imported: " + profiles.Count); // Using LogInfo from BaseService

            var errors = new Dictionary<int, ProfileImportErrorInfo>();
            int i = 0;

            foreach (var profileDto in profiles)
            {
                if (_profileValidator.ValidateProfile(profileDto))
                {
                    var addRes = _profileService.AddProfile(profileDto); // Corrected method name
                    if (!addRes.Status)
                    {
                        errors[i + 1] = new ProfileImportErrorInfo(addRes.Msg ?? "Unknown error", profileDto);
                    }
                }
                else
                {
                    errors[i + 1] = new ProfileImportErrorInfo("Validation Errors", profileDto);
                }

                i++;
            }

            return new ResInfo
            {
                Status = true,
                Data = profiles,
                Desc = "Profiles imported with some errors.",
                Msg = errors.Count > 0 ? "Some profiles had errors." : "All profiles imported successfully."
            };
        }
    }
}

