using System.Text.Json.Serialization;
namespace NewPlasmaDonorsAPI.Dto
{
    public class ResInfo
    {
        //[JsonPropertyName("success")]
        //public bool Success { get; set; } = true; // Default value

        [JsonPropertyName("status")]
        public bool Status { get; set; } = true; // Default value
        [JsonPropertyName("data")]
        public object? Data { get; set; } // Can hold any type of data

        public string? Msg { get; set; } // Message field
        public string? Desc { get; set; } // Description field
    }
}
