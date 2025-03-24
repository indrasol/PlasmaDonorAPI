using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using NewPlasmaDonorsAPI.Services;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Services;

namespace NewPlasmaDonorsAPI.Controllers
{

    [ApiController]
    [Route("api/profile")]
    [EnableCors("AllowAll")]
   // [Authorize]

    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;
        private readonly MdService _mdService;
        private readonly ProfileImportService profileImportService;
        private const string CLASSPATH_URL_PREFIX = "classpath:";

        public ProfileController(ProfileService profileService, MdService mdService,ProfileImportService profileImportService, ProfileImportService ProfileImportService)
        {
            _profileService = profileService;
            _mdService = mdService;
            ProfileImportService=profileImportService;
        }
        [HttpPost("create")]
        public IActionResult AddProfile([FromBody] ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResInfo
                {
                    Status = false,
                    Msg = "Invalid input data",
                    Data = ModelState
                });
            }

            var res = _profileService.AddProfile(profileDto);
            if (res.Status)
            {
                return Ok(res); // Return success response
            }

            return BadRequest(res); // Return error response
        }

        [HttpGet("md/get-all")]
        public IActionResult GetAllMasterData()
        {
            var res = _mdService.GetMasterDataForProfile();
            if (res.Status)
            {
                return Ok(res); // Return success response
            }

            return BadRequest(res); // Return error response
        }

        [HttpGet("influencer/get-all-for-lb")]
        public IActionResult GetAllInfluencersForListBox()
        {
            var res = _profileService.GetInfluencersForLb();
            if (res.Status)
            {
                return Ok(res); // Return success response
            }

            return BadRequest(res); // Return error response
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetProfileList()
        {
            var result = await _profileService.GetProfileList(); // Call to ProfileService method
            if (result.Status)
            {
                return Ok(result); // Return successful response
            }
            return BadRequest(result); // Return error response
        }

        [HttpPut("update/id/{id}")]
        public IActionResult UpdateProfile(long id, [FromBody] ProfileModel profileModel)
        {
            try
            {
                var result = _profileService.UpdateProfile(profileModel, id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message }); // Return 404 if the profile is not found
            }
        }

        [HttpPost("md/get-by-types")]
        public async Task<ActionResult<ResInfo>> GetAllMasterData([FromBody] MdReqInfo mdReq)
        {
            if (mdReq == null || mdReq.Types == null || mdReq.Types.Count == 0)
            {
               return Ok(new Dictionary<string, List<MdInfo>>());
            }

            var result = await _mdService.GetMasterDataByTypesAsync(mdReq.Types);
            return Ok(result);
        }

        [HttpGet("delete/id/{id}")]
        public IActionResult DeleteProfile(long id)
        {
            var result = _profileService.DeleteProfile(id);
            return Ok(result);
        }
        // 📌 Private method inside ProfileController
        private static FileInfo GetFile(string resourceLocation)
        {
            if (string.IsNullOrEmpty(resourceLocation))
            {
                throw new ArgumentNullException(nameof(resourceLocation), "Resource location must not be null");
            }

            if (resourceLocation.StartsWith(CLASSPATH_URL_PREFIX))
            {
                string path = resourceLocation.Substring(CLASSPATH_URL_PREFIX.Length);
                string description = $"Class path resource [{path}]";

                var assembly = typeof(ProfileController).Assembly;
                var resourceStream = assembly.GetManifestResourceStream(path);
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"{description} cannot be resolved to absolute file path because it does not exist");
                }

                // Save resource to a temporary file
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(path));
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
                return new FileInfo(tempFilePath);
            }

            try
            {
                Uri uri = new Uri(resourceLocation);
                if (uri.IsFile)
                {
                    return new FileInfo(uri.LocalPath);
                }
            }
            catch (UriFormatException)
            {
                // Not a URL, treat as file path
            }

            return new FileInfo(resourceLocation);
        }

        [HttpPost("import/excel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportProfiles(IFormFile xlFile)
        {
            if (xlFile == null || xlFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Process the file
            try
            {
                // Call your service to import profiles
                var result = await profileImportService.ImportProfiles(xlFile);

                if (result.Status)
                {
                    return Ok(result); // Return a success response
                }
                else
                {
                    return BadRequest(result.Msg); // Handle error cases
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Handle any exceptions
            }
        }

        // 📌 API Endpoint for downloading the template
        [HttpGet("download/template")]
        public IActionResult DownloadTemplate()
        {
            try
            {

                string filePath = "/home/ubuntu/apps/deploy/static-files/donor_profile_template.xlsx";
                FileInfo file = GetFile(filePath);

                if (!file.Exists)
                {
                    return NotFound("File not found.");
                }

                var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(file.FullName, out string? contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(fileStream, contentType, "donor_profile_template.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving file: {ex.Message}");
            }
        }



    }
}
