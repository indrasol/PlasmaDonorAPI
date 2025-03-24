namespace NewPlasmaDonorsAPI.Models
{
    public class MasterData
    {
        public long id { get; set; }

        public DateTime? createdOn { get; set; }

        public string? mdName { get; set; }

        public string? mdTitle { get; set; }

        public string? mdType { get; set; }

        public string? status { get; set; }

        public DateTime? updatedOn { get; set; }

        public long? createdBy { get; set; }

        public long? updatedBy { get; set; }

        public string? mdScore { get; set; }

        //// Navigation Properties for foreign keys
        public virtual UserModel? createdByUser { get; set; }
        public virtual UserModel? updatedByUser { get; set; }
        //public virtual ProfileMdMap? ProfileMdMaps { get; set; }
    }
}
