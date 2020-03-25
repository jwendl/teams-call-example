using System.Collections.Generic;
using System.Threading.Tasks;

namespace Teams.ConferenceApi.TokenManager.Interfaces
{
    public interface ITokenCreator
    {
        Task<string> GetIntegrationTestTokenAsync();

        Task<string> GetUserBasedAccessTokenAsync();

        Task<string> GetClientApplicationAccessTokenAsync();

        Task<string> GetAccessTokenOnBehalfOf(IEnumerable<string> scopes, string userAssertionToken);
    }
}
