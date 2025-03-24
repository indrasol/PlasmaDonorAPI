using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewPlasmaDonorsAPI.Models
{
    // File: Models/CompanyLocation.cs
    public class CompanyLocation
    {
        public long id { get; set; }
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? city { get; set; }
        public string? country { get; set; }
        public string? countryCode { get; set; }
        public string? fullAddress { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? name { get; set; }
        public string? state { get; set; }
        public string? stateCode { get; set; }
        public string? status { get; set; }
        public string? phoneNumber { get; set; }
        public string? siteId { get; set; }

        
    }
}
