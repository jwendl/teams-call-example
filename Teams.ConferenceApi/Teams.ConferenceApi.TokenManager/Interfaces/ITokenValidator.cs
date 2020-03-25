using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Teams.ConferenceApi.TokenManager.Interfaces
{
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue authenticationHeaderValue);

        Task<ClaimsPrincipal> ValidateTokenAsync(string token);
    }
}
