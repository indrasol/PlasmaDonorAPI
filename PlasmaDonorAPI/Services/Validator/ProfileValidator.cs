using NewPlasmaDonorsAPI.Dto;
using System.Text.RegularExpressions;

namespace NewPlasmaDonorsAPI.Services.Validator
{
    public class ProfileValidator
    {

        // Validate first name
        public bool IsValidFirstName(string firstName)
        {
            return !string.IsNullOrEmpty(firstName) && Regex.IsMatch(firstName, "^[a-zA-Z]{2,}$");
        }

        // Validate last name
        public bool IsValidLastName(string lastName)
        {
            return !string.IsNullOrEmpty(lastName) && Regex.IsMatch(lastName, "^[a-zA-Z]{2,}$");
        }

        // Validate email
        public bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, "^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+$");
        }

        // Validate phone number
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            // Example criteria: valid phone number with 10 digits
            return !string.IsNullOrEmpty(phoneNumber) && Regex.IsMatch(phoneNumber, @"\d{10}");
        }

        // Validate profile credentials
        public bool ValidateProfile(ProfileDto profile)
        {
            return
                profile.email != null && IsValidEmail(profile.email) &&
                profile.firstName != null && IsValidFirstName(profile.firstName) &&
                profile.lastName != null && IsValidLastName(profile.lastName) &&
                profile.phoneNumber != null && IsValidPhoneNumber(profile.phoneNumber);
        }
    }
}
