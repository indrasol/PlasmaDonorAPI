namespace NewPlasmaDonorsAPI.Models
{
    public class SecUserDetails : UserModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string RoleType { get; set; }
        public string Status { get; set; }
        public long roleId { get; set; }
        public string Email { get; set; }

        //public string Company { get; set; }
        public long CompanyId { get; set; }

        public string CompanyLocation { get; set; }
        public long CompanyLocationId { get; set; }

        public bool Deleted { get; set; } = false;

        public bool AccountNonExpired { get; set; }
        public bool AccountNonLocked { get; set; }
        public bool CredentialsNonExpired { get; set; }
        public bool Enabled { get; set; }

        // Constructor
        public SecUserDetails()
        {
            Deleted = false; // Default value
        }

        // If implementing role-based authorization, consider returning roles
        public virtual ICollection<string> GetAuthorities()
        {
            return new List<string>(); // Placeholder for user roles
        }
    }
}

