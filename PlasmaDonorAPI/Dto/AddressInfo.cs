namespace NewPlasmaDonorsAPI.Dto
{
    public class AddressInfo
    {
        public long Id { get; set; }
        public string AddressLine { get; set; }
        public string FullAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string PostalCode { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }

        public AddressInfo() { }

        public AddressInfo(
            long id, string addressLine, string fullAddress, string city, string state,
            string stateCode, string country, string countryCode, double? latitude, double? longitude,
            string postalCode, string status, string createdOn, string updatedOn)
        {
            Id = id;
            AddressLine = addressLine;
            FullAddress = fullAddress;
            City = city;
            State = state;
            StateCode = stateCode;
            Country = country;
            CountryCode = countryCode;
            Latitude = latitude;
            Longitude = longitude;
            PostalCode = postalCode;
            Status = status;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }
    }
}
