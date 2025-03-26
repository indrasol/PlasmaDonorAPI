namespace NewPlasmaDonorsAPI.Dto.Dashboard
{
    [Serializable]
    public class KpiInfo
    {
        public string? title { get; set; }
        public string? valStr { get; set; }
        public string? bgColor { get; set; }
        public string? icon { get; set; }
        public double value { get; set; }
        public long cont { get; set; }
    }
}
