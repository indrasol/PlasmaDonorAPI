using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Repositories;

namespace NewPlasmaDonorsAPI.Services
{
    public interface IUserService
    {
       
        Task<UserModel?> FindByEmailAsync(string email);

        Task<ResInfo> LoginAsync(LoginDto user);
        Task<ResInfo> AddUserAsync(UserDto userDto);
        Task<List<UserDto>> FindAllEmployeesAsync();
        Task<List<UserModel>> GetEmployeeListAsync();
        Task<ResInfo> GetCompanyLocationsAsync();
        Task<ResInfo> GetRolesAsync();
        Task<string> UpdateUserAsync(UserModel updatedModel, long id);
        Task<ResInfo> DeleteUserAsync(long id);
        //Task<UserModel?> FindByIdAsync(long id);

        Task<UserModel?> FetchUserDetailsByIdAsync(long id);

        //public Task<List<UserModel>> FindAllUsersByDeletedFalse();
        //List<UserModel> FindAllUsersByDeletedFalseAsync();



    }
}
