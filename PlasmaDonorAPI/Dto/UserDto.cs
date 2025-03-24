using Newtonsoft.Json;

namespace NewPlasmaDonorsAPI.Dto
{
    public class UserDto
    {
        public long? id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public long? companyLocationId { get; set; }
        public long? roleId { get; set; }
        public string? role { get; set; }
        public string? roleType { get; set; }
        public string? status { get; set; }
        public string? company { get; set; }

        public long? companyId { get; set; }
        public string? companyLocation { get; set; }
       
        public string? createdOn { get; set; }
        public string? updatedOn { get; set; }

        public string? username { get; set; }
        public bool accountNonExpired { get; set; }
        public bool accountNonLocked { get; set; }
        public bool credentialsNonExpired { get; set; }
        public bool enabled { get; set; }
        public bool deleted { get; set; } = false;

        public string? token { get; set; }
    }
}
