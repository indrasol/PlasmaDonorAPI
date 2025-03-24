namespace NewPlasmaDonorsAPI.Dto
{
    public class CountDto
    {
        public int Count { get; set; }         // The count of profiles
        //public string Category { get; set; }   // Can be State, Occupation, Relationship, Education, etc.
        public string? State { get; set; }     // State name (if applicable)
        //public string? Occupation { get; set; } // Occupation (if applicable)
        //public string? Relationship { get; set; } // Relationship status (if applicable)
        //public string? Education { get; set; } // Education level (if applicable)
    }
}
