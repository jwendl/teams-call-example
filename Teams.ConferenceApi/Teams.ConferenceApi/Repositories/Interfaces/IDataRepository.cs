using System.Collections.Generic;
using System.Threading.Tasks;

namespace Teams.ConferenceApi.Repositories.Interfaces
{
    public interface IDataRepository<T>
    {
        Task<T> CreateItemAsync(T item);

        Task<IEnumerable<T>> FetchAllUsersAsync();

        Task<T> FetchItemByIdAsync(string id);

        Task DeleteItemByIdAsync(string id);
    }
}
