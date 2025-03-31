using System.Text.Json.Serialization;

namespace NewPlasmaDonorsAPI.Dto
{
    public class ResInfo
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; } = true; //Default to true for successful response

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("msg")]
        public string? Msg { get; set; } //Message field

        [JsonPropertyName("desc")]
        public string? Desc { get; set; } //Description field

        //// Default constructor (success response)
        //public ResInfo() { }

        //// Constructor for custom success response
        //public ResInfo(object data, string? message = null)
        //{
        //    Status = true;
        //    Data = data;
        //    Msg = message;
        //}

        //// Constructor for error response
        //public ResInfo(string message, string? description = null)
        //{
        //    Status = false;
        //    Msg = message;
        //    Desc = description;
        //}
    }
}
