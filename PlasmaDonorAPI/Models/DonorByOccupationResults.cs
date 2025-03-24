using Microsoft.EntityFrameworkCore;

namespace NewPlasmaDonorsAPI.Models
{
    [Keyless]
    public class DonorByOccupationResults
    {
        public int id { get; set; }
        public string? MdTitle { get; set; }  // Represents 'b.md_title'
        public int Cnt { get; set; }  // Represents 'COUNT(a.id)'
    }
}
