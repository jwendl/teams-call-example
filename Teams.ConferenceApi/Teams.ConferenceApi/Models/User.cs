using System.Text.Json.Serialization;

namespace Teams.ConferenceApi.Models
{
    public class User
        : BaseModel
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("meetingUrl")]
        public string MeetingUrl { get; set; }
    }
}
