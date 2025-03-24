using Microsoft.VisualStudio.Shell.Interop;
using NewPlasmaDonorsAPI.Dto;
using Microsoft.Extensions.Logging;
using NewPlasmaDonorsAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NewPlasmaDonorsAPI.Services
{

  
    public class BaseService
    {

        protected readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(ILogger<BaseService> logger, IHttpContextAccessor httpContextAccessor)
            {
                _logger = logger;
                _httpContextAccessor = httpContextAccessor;
            }
            
         
        // Success response
        protected ResInfo Success(object data)
        {
            return new ResInfo
            {
                Data = data,
                Msg = "success",
                //Success = true,
                Status= true
            };
        }

        public SecUserDetails GetLoggedInUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                return new SecUserDetails
                {
                    Id = (long)GetLoggedUserId(),
                    Username = user.Identity.Name // Assuming Name is set in the claims
                };
            }

            return null;
        }

        public long? GetLoggedUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return idClaim != null ? long.Parse(idClaim.Value) : (long?)null;
        }


        // Error response with message only
        protected ResInfo Error(string msg)
        {
            return Error(msg,data: null);
        }

        // Error response with message and data
        protected ResInfo Error(string msg, object data)
        {
            return Error(msg, data, null!);
        }

        // Error response with message, data, and exception details
        protected ResInfo Error(string msg, object data, Exception e)
        {
            var resInfo = new ResInfo
            {
                Data = data,
                Msg = msg,
                Status = false
            };

            if (e != null)
            {
                // Add exception stack trace to data if exception is provided
                resInfo.Data = e.StackTrace!;
            }

            return resInfo;
        }
    }

   
}
