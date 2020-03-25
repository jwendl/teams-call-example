using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Teams.ConferenceApi.Repositories.Interfaces;
using Teams.ConferenceApi.TokenManager.Interfaces;

namespace Teams.ConferenceApi.Repositories
{
    public class MicrosoftGraphRepository
        : IMicrosoftGraphRepository
    {
        private readonly ITokenCreator tokenCreator;

        public MicrosoftGraphRepository(ITokenCreator tokenCreator)
        {
            this.tokenCreator = tokenCreator ?? throw new ArgumentNullException(nameof(tokenCreator));
        }

        public async Task<User> FetchMeAsync(AuthenticationHeaderValue authenticationHeaderValue)
        {
            _ = authenticationHeaderValue ?? throw new ArgumentNullException(nameof(authenticationHeaderValue));

            var graphServiceClient = BuildGraphServiceClient(authenticationHeaderValue);
            return await graphServiceClient.Me
                .Request()
                .GetAsync();
        }

        public async Task<OnlineMeeting> CreateOnlineMeetingAsync(AuthenticationHeaderValue authenticationHeaderValue)
        {
            _ = authenticationHeaderValue ?? throw new ArgumentNullException(nameof(authenticationHeaderValue));

            var graphServiceClient = BuildGraphServiceClient(authenticationHeaderValue);

            var onlineMeeting = new OnlineMeeting()
            {
                StartDateTime = DateTime.Now.AddHours(-1),
                EndDateTime = DateTime.Now.AddHours(5),
            };

            return await graphServiceClient.Me.OnlineMeetings
                .Request()
                .AddAsync(onlineMeeting);
        }

        private GraphServiceClient BuildGraphServiceClient(AuthenticationHeaderValue authenticationHeaderValue)
        {
            _ = authenticationHeaderValue ?? throw new ArgumentNullException(nameof(authenticationHeaderValue));

            var delegateAuthenticationProvider = new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    var scopes = new List<string>() { "https://graph.microsoft.com/User.Read", "https://graph.microsoft.com/OnlineMeetings.ReadWrite" };
                    var userToken = await tokenCreator.GetAccessTokenOnBehalfOf(scopes, authenticationHeaderValue.Parameter);

                    // Append the access token to the request.
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", userToken);
                });

            var graphServiceClient = new GraphServiceClient(delegateAuthenticationProvider);
            return graphServiceClient;
        }
    }
}
