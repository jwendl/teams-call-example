using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Teams.ConferenceApi.Repositories.Interfaces
{
    public interface IDocumentationRepository
    {
        Task<string> GetSwaggerDocumentContentAsync(HttpRequest httpRequest, Assembly assembly);

        Task<string> GetSwaggerUIContentAsync(HttpRequest httpRequest, string documentName, string authCode, Assembly assembly);
    }
}
