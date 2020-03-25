using System;
using System.Text.Json.Serialization;

namespace Teams.ConferenceApi.Models
{
    public class BaseModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }
}
