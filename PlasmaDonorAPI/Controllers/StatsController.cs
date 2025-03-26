using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Services;
using Serilog;

namespace NewPlasmaDonorsAPI.Controllers
{
    [ApiController]
    [Route("api/stats")]
    [EnableCors("AllowAll")] 
    public class StatsController : ControllerBase
    {
        private readonly StatService _statService;
        private readonly ILogger<StatsController> _logger;
        private readonly AppDbContext _context; // Add DbContext as a dependency

        public StatsController(StatService statService, ILogger<StatsController> logger, AppDbContext context)
        {
            _statService = statService;
            _logger = logger;
            _context = context; // Initialize DbContext
        }

       
        [HttpGet("dashboard")]
        public async Task<ActionResult<ResInfo>> GetDashboardStats()
        {
            var result = await _statService.GetDashboardStatsAsync(); // ✅ Await the async method
            return Ok(result);
        }




        [HttpPost("inf-tree-data")]
        public async Task<ActionResult<ResInfo>> InfTreeData([FromBody] ProfileDto pDto)
        {
            var result = await _statService.InfluencerTreeData(pDto);
            return Ok(result);
        }

        [HttpPost("prof-stats-data")]
        public ActionResult<ResInfo> ProfStatsData([FromBody] ProfileDto pDto)
        {
            return _statService.ProfileStatsData(pDto);
        }

        [HttpPost("search-profiles")]
        public ActionResult<ResInfo> InfDetailsData([FromBody] ProfileDto pDto)
        {
            _logger.LogInformation("::: {ProfileData}", pDto);
            return _statService.InfDetailsData(pDto);
        }

        [HttpPost("donor-heatm-data")]
        public ActionResult<ResInfo> DonorStatsHmData([FromBody] ProfileDto pDto)
        {
            return _statService.DonorStatsHmData(pDto);
        }
    }
}
