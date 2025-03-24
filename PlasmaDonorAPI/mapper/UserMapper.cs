using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.mapper
{
    public class UserMapper
    {
        public static UserDto MapToUserDto(UserModel model)
        {
            return new UserDto
            {
                firstName = model.firstName,
                lastName = model.lastName,
                email = model.email,
                phoneNumber = model.phoneNumber,
                roleType = model.roleType,
                password = model.password,
                username = model.username,
                deleted = model.deleted ?? false // Fix for CS0266 and CS8629
            };
        }

        public static UserModel MapToUserModel(UserDto info)
        {
            return new UserModel
            {
                firstName = info.firstName,
                lastName = info.lastName,
                email = info.email,
                phoneNumber = info.phoneNumber,
                roleType = info.roleType,
                password = info.password,
                username = info.username,
                deleted = info.deleted,
                status = info.status,
            };
        }

        public static UserModel MapToUserModel(SecUserDetails info)
        {
            return new UserModel
            {
                id = info.Id,
                firstName = info.FirstName,
                lastName = info.LastName,
                email = info.Email,
                phoneNumber = info.PhoneNumber,
                roleType = info.RoleType,
                password = info.Password,
                username = info.Username,
                deleted = info.Deleted,
                status = info.Status
            };
        }

        public static SecUserDetails MapToSecUser(UserModel info)
        {
            return new SecUserDetails
            {
                Id = info.id,
                FirstName = info.firstName,
                LastName = info.lastName,
                Email = info.email,
                PhoneNumber = info.phoneNumber,
                RoleType = info.roleType,
                Password = info.password,
                Username = info.email, // Keeping same logic as Java version
                Deleted = info.deleted ?? false, // Fix for CS0266 and CS8629
                Status = info.status
            };
        }
    }
}
