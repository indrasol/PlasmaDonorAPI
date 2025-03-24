namespace NewPlasmaDonorsAPI.Dto
{
    public class CompanyLocationInfo
    {
        public long Id { get; set; }
        public string SiteId { get; set; }
        public string Company { get; set; }
        public long CompanyId { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string FullAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; }

        public CompanyLocationInfo() { }

        public CompanyLocationInfo(
            long id, string siteId, string company, long companyId, string phoneNumber,
            string addressLine1, string addressLine2, string fullAddress, string city, string state,
            string stateCode, string country, string countryCode, double? latitude, double? longitude, string status)
        {
            Id = id;
            SiteId = siteId;
            Company = company;
            CompanyId = companyId;
            PhoneNumber = phoneNumber;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            FullAddress = fullAddress;
            City = city;
            State = state;
            StateCode = stateCode;
            Country = country;
            CountryCode = countryCode;
            Latitude = latitude;
            Longitude = longitude;
            Status = status;
        }
    }
}
