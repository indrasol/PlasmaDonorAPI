using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.mapper
{
    public class ProfileMapper
    {
        public static ProfileDto? MapToProfileDto(ProfileModel? model)
        {
            if (model == null) return null;

            return new ProfileDto
            {
                dob = model.dob,
                email = model.email,
                firstName = model.firstName,
                lastName = model.lastName,
                gender = model.gender,
                isDonor = model.isDonor,
                isInfluencer = model.isInfluencer,
                phoneNumber = model.phoneNumber
            };
        }

        // Map from ProfileDto to ProfileModel
        public static ProfileModel? MapToProfileModel(ProfileDto? info)
        {
            if (info == null) return null;

            return new ProfileModel
            {
                dob = info.dob,
                email = info.email,
                firstName = info.firstName,
                lastName = info.lastName,
                gender = info.gender,
                isDonor = info.isDonor,
                isInfluencer = info.isInfluencer,
                phoneNumber = info.phoneNumber
            };
        }
    }
}
