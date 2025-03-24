using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Repositories;
using System.Threading.Tasks;

namespace NewPlasmaDonorsAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<UserModel> GetAllUsers()
        {
            return _context.Users.ToList();  // Fetch users from database
        }
        public async Task<UserModel?> FindByEmailAsync(string email)
        {
            if (_context.Users == null)
            {
                return null;
            }

            return await _context.Users
                .FirstOrDefaultAsync(user => user.email == email);
        }

        public async Task<List<UserModel>> FindAllByRoleTypeAndDeletedAsync()
        {
            if (_context.Users == null)
            {
                return new List<UserModel>();
            }

            return await _context.Users
                //.Where(user => user.role == "ADMIN" && user.deleted == false)
                .ToListAsync();
        }
      
        public Task<List<UserModel>> FindTopPerformerAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddUserAsync(UserModel user)
        {
            if (_context.Users == null)
            {
                throw new InvalidOperationException("Users DbSet is not initialized.");
            }

            // Add the new user to the User DbSet
            await _context.Users.AddAsync(user);

            // Save the changes asynchronously to the database
            await _context.SaveChangesAsync();
        }
        public List<UserModel> FindAllUsersByDeletedFalse()
        {
            if (_context.Users == null)
            {
                return new List<UserModel>();
            }

            return _context.Users.Where(u => u.deleted == false).ToList();
        }

       
        public UserModel? GetById(long? id)
        {
            if (_context.Users == null)
            {
                throw new InvalidOperationException("Users DbSet is not initialized.");
            }

            var user = _context.Users.FirstOrDefault(u => u.id == id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }

            return user;
        }
   
        public UserModel Save(UserModel userModel)
        {
            var existingUser = _context.Users!.FirstOrDefault(u => u.id == userModel.id);
            if (existingUser == null)
            {
                _context.Users.Add(userModel);
            }
            else
            {
                _context.Entry(existingUser).CurrentValues.SetValues(userModel);
            }
            
            _context.SaveChanges();
            return userModel;
        }

        // Get the count of active users
        public long GetActiveUserCount()
        {
            //return _context.Users.Count(u => u.Deleted == false || u.Deleted == null);
            if (_context.Users == null)
            {
                return 0;
            }

            return _context.Users.Count(u => (u.deleted == false || u.deleted == null) && u.role == "Employee");
        }

        // Get the count of active employees
        public long GetActiveEmployeesCount()
        {
            // return _context.Users.Count(u => (u.Deleted == false || u.Deleted == null) && u.Role == "Employee");
            if (_context.Users == null)
            {
                return 0;
            }

            return _context.Users.Count(u => (u.deleted == false || u.deleted == null) && u.role == "Employee");
        }
        
        // Get the count of admins
        public long GetAdminCount()
        {
            //return _context.Users.Count(u => (u.Deleted == false || u.Deleted == null) && u.Role == "Admin");
            if (_context.Users == null)
            {
                return 0;
            }

            return _context.Users.Count(u => (u.deleted == false || u.deleted == null) && u.role == "Admin");
        }

    }
}
