using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.Repositories
{
    
public class CompanyLocationRepository : ICompanyLocationRepository
    {
        private readonly AppDbContext _context;

        public CompanyLocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public AppDbContext Get_context()
        {
            return _context;
        }

        public IEnumerable<CompanyLocation> GetAllLocations(AppDbContext _context)
        {
            return _context.companyLocation?.ToList() ?? new List<CompanyLocation>();
        }

        public CompanyLocation? GetById(long? id)
        {
            return _context.companyLocation.FirstOrDefault(cl => cl.id == id);
        }
    }
}
