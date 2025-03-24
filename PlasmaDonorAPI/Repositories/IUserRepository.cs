using NewPlasmaDonorsAPI.Models;
using System.Threading.Tasks;

namespace NewPlasmaDonorsAPI.Repositories
{
    public interface IUserRepository
    {
        List<UserModel> GetAllUsers();
        Task<UserModel?> FindByEmailAsync(string Email);
        //Task<List<UserModel>> FindByRoleEmployeeAsync();
        Task<List<UserModel>> FindTopPerformerAsync();
        Task AddUserAsync(UserModel user);
        UserModel? GetById(long? id);
        UserModel Save(UserModel user);
        long GetActiveUserCount();
        long GetActiveEmployeesCount();
        long GetAdminCount();
        List<UserModel> FindAllUsersByDeletedFalse();

        Task<List<UserModel>> FindAllByRoleTypeAndDeletedAsync();
        
        //Task UpdateAsync(UserModel user); // Add this method to the interface
    }
}
