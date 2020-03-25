using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Teams.ConferenceApi.Models;
using Teams.ConferenceApi.Repositories.Interfaces;
using Teams.ConferenceApi.TokenManager.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Teams.ConferenceApi
{
    public class ConferenceCallFunctions
    {
        private readonly IJsonTextSerializer jsonTextSerializer;
        private readonly ITokenValidator tokenValidator;
        private readonly IDataRepository<User> userRepository;
        private readonly IMicrosoftGraphRepository microsoftGraphRepository;

        public ConferenceCallFunctions(IJsonTextSerializer jsonTextSerializer, ITokenValidator tokenValidator, IDataRepository<User> userRepository, IMicrosoftGraphRepository microsoftGraphRepository)
        {
            this.jsonTextSerializer = jsonTextSerializer ?? throw new ArgumentNullException(nameof(jsonTextSerializer));
            this.tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.microsoftGraphRepository = microsoftGraphRepository ?? throw new ArgumentNullException(nameof(microsoftGraphRepository));
        }

        [FunctionName(nameof(CreateConferenceCall))]
        [OpenApiOperation(nameof(CreateConferenceCall), "Create Call", Description = "Creates a call meeting.")]
        [OpenApiParameter("Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string))]
        [OpenApiRequestBody("application/json", typeof(User), Description = "A user object.")]
        [OpenApiResponseBody(HttpStatusCode.Created, "application/json", typeof(User))]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
        public async Task<HttpResponseMessage> CreateConferenceCall([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "calls")] HttpRequestMessage req, ILogger log)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            var claimsPrincipal = await tokenValidator.ValidateTokenAsync(req.Headers.Authorization);
            if (claimsPrincipal == null) return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            log.LogInformation("Creating call");
            var contentStream = await req.Content.ReadAsStreamAsync();
            var jsonResult = await jsonTextSerializer.DeserializeObjectAsync<User>(contentStream);

            var onlineMeeting = await microsoftGraphRepository.CreateOnlineMeetingAsync(req.Headers.Authorization);
            jsonResult.MeetingUrl = onlineMeeting.JoinWebUrl;

            var content = new StringContent(JsonSerializer.Serialize(jsonResult), Encoding.UTF8, "application/json");
            log.LogInformation("Created call");
            return new HttpResponseMessage(HttpStatusCode.Created) { Content = content }; ;
        }
    }
}
