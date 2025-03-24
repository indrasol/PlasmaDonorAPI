using NewPlasmaDonorsAPI.Dto;

namespace NewPlasmaDonorsAPI.Dto
{
    public class ProfileImportErrorInfo
    {
        // Properties
        public string ErrorMsg { get; set; }
        public ProfileDto Profile { get; set; }

        // Constructors
        //public ProfileImportErrorInfo() { }

        public ProfileImportErrorInfo(string errorMsg, ProfileDto profile)
        {
            ErrorMsg = errorMsg;
            Profile = profile;
        }
    }
}
