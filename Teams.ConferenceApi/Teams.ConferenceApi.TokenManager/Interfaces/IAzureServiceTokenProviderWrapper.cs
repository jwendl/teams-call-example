using System.Threading.Tasks;

namespace Teams.ConferenceApi.TokenManager.Interfaces
{
    public interface IAzureServiceTokenProviderWrapper
    {
        Task<string> GetAccessTokenAsync(string resource);
    }
}
