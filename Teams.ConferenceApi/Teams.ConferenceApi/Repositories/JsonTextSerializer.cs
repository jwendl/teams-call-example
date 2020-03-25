using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Teams.ConferenceApi.Repositories.Interfaces;

namespace Teams.ConferenceApi.Repositories
{
    public class JsonTextSerializer
        : IJsonTextSerializer
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public JsonTextSerializer()
        {
            jsonSerializerOptions = new JsonSerializerOptions();
        }

        public async Task<T> DeserializeObjectAsync<T>(Stream contentStream)
        {
            return await JsonSerializer.DeserializeAsync<T>(contentStream, jsonSerializerOptions);
        }

        public string SerializeObjectAsync<T>(T item)
        {
            return JsonSerializer.Serialize(item, jsonSerializerOptions);
        }

        public async Task SerializeObjectAsync<T>(MemoryStream memoryStream, T item)
        {
            await JsonSerializer.SerializeAsync<T>(memoryStream, item);
            memoryStream.Position = 0;
        }
    }
}
