using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using NewPlasmaDonorsAPI.Services;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Services;
using EnvDTE;

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
        private readonly ProfileImportService _profileImportService;
        private readonly IWebHostEnvironment _env;
        public ProfileController(ProfileService profileService, MdService mdService, ProfileImportService ProfileImportService, IWebHostEnvironment env)
        {
            _profileService = profileService;
            _mdService = mdService;
            _profileImportService=ProfileImportService;
            _env = env;
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
        public IActionResult GetAllInfluencersForListBox([FromQuery] int? hc)
        {
            var res = _profileService.GetInfluencersForLb(hc);
            if (res.Status)
            {
                return Ok(res); // Return success response
            }

            return BadRequest(res); // Return error response
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetProfileList()
        {
            var result = await _profileService.GetProfileListNew(); // Call to ProfileService method
            if (result!=null)
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
                var result = await _profileImportService.ImportProfiles(xlFile);
                
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

                string filePath = Path.Combine(_env.WebRootPath,"files","donor_profile_template.xlsx");
                FileInfo file = new FileInfo(filePath);

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
