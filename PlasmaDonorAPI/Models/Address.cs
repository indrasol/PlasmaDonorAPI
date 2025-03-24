namespace NewPlasmaDonorsAPI.Models
{
    public class Address
    {
        public long id { get; set; }

        public string? addressLine { get; set; }

        public string? city { get; set; }

        public string? country { get; set; }

        public string? countryCode { get; set; }

        public DateTime? createdOn { get; set; }

        public string? fullAddress { get; set; }

        public double? latitude { get; set; }

        public double? longitude { get; set; }

        public string? state { get; set; }

        public string? stateCode { get; set; }

        public string? status { get; set; }

        public DateTime? updatedOn { get; set; }

        public long? createdBy { get; set; }

        public long? updatedBy { get; set; }

        public string? postalCode { get; set; }

        // Navigation Properties for foreign keys
        public virtual UserModel? createdByUser { get; set; }

        public virtual UserModel? updatedByUser { get; set; }
    }
}
