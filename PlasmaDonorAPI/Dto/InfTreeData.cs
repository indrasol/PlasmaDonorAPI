namespace NewPlasmaDonorsAPI.Dto
{
    public class InfTreeData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<InfTreeData> Childs { get; set; }

        public InfTreeData()
        {
            Childs = new List<InfTreeData>();
        }

        public InfTreeData(long id, string name, string email, List<InfTreeData> childs)
        {
            Id = id;
            Name = name;
            Email = email;
            Childs = childs ?? new List<InfTreeData>();
        }
    }
}
