using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
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
    public class UserFunctions
    {
        private readonly IJsonTextSerializer jsonTextSerializer;
        private readonly ITokenValidator tokenValidator;
        private readonly IDataRepository<User> userRepository;

        public UserFunctions(IJsonTextSerializer jsonTextSerializer, ITokenValidator tokenValidator, IDataRepository<User> userRepository)
        {
            this.jsonTextSerializer = jsonTextSerializer ?? throw new ArgumentNullException(nameof(jsonTextSerializer));
            this.tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [FunctionName(nameof(CreateUser))]
        [OpenApiOperation(nameof(CreateUser), "Create a User", Description = "Creates a user.")]
        [OpenApiParameter("Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string))]
        [OpenApiRequestBody("application/json", typeof(User), Description = "The new user information.")]
        [OpenApiResponseBody(HttpStatusCode.Created, "application/json", typeof(User))]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
        public async Task<HttpResponseMessage> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequestMessage req, ILogger log)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            log.LogInformation("Creating user");
            var contentStream = await req.Content.ReadAsStreamAsync();
            var jsonResult = await jsonTextSerializer.DeserializeObjectAsync<User>(contentStream);

            var createdItem = await userRepository.CreateItemAsync(jsonResult);
            var content = new StringContent(JsonSerializer.Serialize(createdItem), Encoding.UTF8, "application/json");
            log.LogInformation("Created user");
            return new HttpResponseMessage(HttpStatusCode.Created) { Content = content }; ;
        }

        [FunctionName(nameof(ListUsers))]
        [OpenApiOperation(nameof(ListUsers), "List Users", Description = "Gets a list of users.")]
        [OpenApiParameter("Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.Created, "application/json", typeof(IEnumerable<User>))]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
        public async Task<HttpResponseMessage> ListUsers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequestMessage req, ILogger log)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            log.LogInformation("Getting user list");
            var createdItem = await userRepository.FetchAllUsersAsync();
            var content = new StringContent(JsonSerializer.Serialize(createdItem), Encoding.UTF8, "application/json");
            log.LogInformation("Getting user list");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content }; ;
        }

        [FunctionName(nameof(ReadUser))]
        [OpenApiOperation(nameof(ReadUser), "Read User", Description = "Reads a user.")]
        [OpenApiParameter("Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.Created, "application/json", typeof(IEnumerable<User>))]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
        public async Task<HttpResponseMessage> ReadUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}")] HttpRequestMessage req, ILogger log, string userId)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            log.LogInformation("Getting user list");
            var item = await userRepository.FetchItemByIdAsync(userId);
            var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
            log.LogInformation("Getting user list");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        }

        [FunctionName(nameof(DeleteUser))]
        [OpenApiOperation(nameof(DeleteUser), "Deletes User", Description = "Deletes a user.")]
        [OpenApiParameter("Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.Created, "application/json", typeof(IEnumerable<User>))]
        [OpenApiResponseBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
        public async Task<HttpResponseMessage> DeleteUser([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{userId}")] HttpRequestMessage req, ILogger log, string userId)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            log.LogInformation("Getting user list");
            await userRepository.DeleteItemByIdAsync(userId);
            log.LogInformation("Getting user list");
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
