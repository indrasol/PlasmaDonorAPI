using Microsoft.EntityFrameworkCore;

namespace NewPlasmaDonorsAPI.Models
{
    [Keyless]  // Not a database table
    public class DonorTimeSeriesResult
    {
        public string Title { get; set; } // DATE_FORMAT(created_on, '%Y-%m-%d')
        public int Cont { get; set; } // count(id)
    }
}
