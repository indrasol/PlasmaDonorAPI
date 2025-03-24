using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.Repositories
{
    public interface ICompanyLocationRepository
    {
        IEnumerable<CompanyLocation> GetAllLocations(AppDbContext _context);
        AppDbContext Get_context();
        CompanyLocation? GetById(long? id);
        //UserModel GetById(long id);
        //long? GetById(long? companyLocationId);
    }
}
