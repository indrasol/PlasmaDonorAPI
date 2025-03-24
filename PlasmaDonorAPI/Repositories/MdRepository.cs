using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Models;

namespace NewPlasmaDonorsAPI.Repositories
{
    public class MdRepository
    {
        private readonly AppDbContext _context;

        public MdRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieve all MasterData by MdType
        public List<MasterData> FindAllByMdType(string mdType)
        {
            return _context.MasterData?
                           .Where(m => m.mdType == mdType)
                           .ToList() ?? new List<MasterData>();
        }
        
        public List<MasterData> FindAllByMdTypeAndMdNameAsync(string mdType, string mdName)
        {
            return  _context.MasterData?
                .Where(md => md.mdType == mdType && md.mdName == mdName)
                .ToList() ?? new List<MasterData>();
        }
    }

}
