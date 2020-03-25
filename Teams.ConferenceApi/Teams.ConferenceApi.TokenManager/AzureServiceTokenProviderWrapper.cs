using System.Threading.Tasks;
using Teams.ConferenceApi.TokenManager.Interfaces;
using Microsoft.Azure.Services.AppAuthentication;

namespace Teams.ConferenceApi.TokenManager
{
    public class AzureServiceTokenProviderWrapper
        : IAzureServiceTokenProviderWrapper
    {
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;

        public AzureServiceTokenProviderWrapper()
        {
            azureServiceTokenProvider = new AzureServiceTokenProvider();
        }

        public async Task<string> GetAccessTokenAsync(string resource)
        {
            return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
        }
    }
}
