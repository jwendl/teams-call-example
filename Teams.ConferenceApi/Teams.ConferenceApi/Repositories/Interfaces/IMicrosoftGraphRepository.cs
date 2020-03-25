using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Teams.ConferenceApi.Repositories.Interfaces
{
    public interface IMicrosoftGraphRepository
    {
        Task<User> FetchMeAsync(AuthenticationHeaderValue authenticationHeaderValue);

        Task<OnlineMeeting> CreateOnlineMeetingAsync(AuthenticationHeaderValue authenticationHeaderValue);
    }
}
