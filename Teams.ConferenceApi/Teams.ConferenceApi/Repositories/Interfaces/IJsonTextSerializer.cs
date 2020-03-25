using System.IO;
using System.Threading.Tasks;

namespace Teams.ConferenceApi.Repositories.Interfaces
{
    public interface IJsonTextSerializer
    {
        Task<T> DeserializeObjectAsync<T>(Stream contentStream);

        string SerializeObjectAsync<T>(T item);

        Task SerializeObjectAsync<T>(MemoryStream memoryStream, T item);
    }
}
