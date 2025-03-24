using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace NewPlasmaDonorsAPI.Models
{
    public class UserModel
    {
        [Key]
        public long id { get; set; }

        [MaxLength(255)]
        public string? firstName { get; set; }

        [MaxLength(255)]
        public string? lastName { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string? email { get; set; }

        [MaxLength(255)]
        public string? phoneNumber { get; set; }

        [MaxLength(255)]
        public string? roleType { get; set; }
       

        [MaxLength(255)]
        public string? password { get; set; }

        [MaxLength(255)]
        public string? username { get; set; }

        [MaxLength(255)]
        public string? status { get; set; }
        public bool? deleted { get; set; }

        [MaxLength(255)]
        public string? role { get; set; }

        public long? company{ get; set; }
       
        public long? roleId { get; set; }

        public long? companyLocation { get; set; }
        public DateTime? createdOn { get; set; }

        public DateTime? updatedOn { get; set; }

        public long? createdBy { get; set; }

        public long? updatedBy { get; set; }

        //// Navigation Properties
        [JsonIgnore]
        public virtual Company? Company { get; set; }
        [JsonIgnore]
        public virtual CompanyLocation? CompanyLocation { get; set; }
        [JsonIgnore]
        public virtual Roles? RoleNavigation { get; set; }
    }
}
