namespace NewPlasmaDonorsAPI.Dto
{
    public class MdInfo
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Parameterless constructor (needed for serialization)
        public MdInfo() { }

        // Constructor with parameters
        public MdInfo(long id, string name, string status)
        {
            Id = id;
            Name = name;
            Status = status;
        }
    }
}
