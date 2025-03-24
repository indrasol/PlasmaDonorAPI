using Microsoft.EntityFrameworkCore;

namespace NewPlasmaDonorsAPI.Models
{
    [Keyless]
    public class ProfileByStateResult
    {
        public string Str { get; set; }  // Represents 'adr.state'
        public int Cnt { get; set; }  // Represents 'COUNT(a.id)'
    }
}
