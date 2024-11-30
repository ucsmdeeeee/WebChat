using System.Text.Json.Serialization;

namespace WebApplication13.Models
{
    public class Log
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("request")]
        public string Request { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("bytesSent")]
        public long BytesSent { get; set; }
    }


}
