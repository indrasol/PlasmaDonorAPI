using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<Roles> GetAllRoles();
       Roles? GetById(long? id);
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Roles> GetAllRoles()
        {
            return _context.roles?.ToList() ?? new List<Roles>();
        }

        public Roles? GetById(long? id)
        {           
            return _context.roles?.First(u => u.id == id);
        }

    }
}
