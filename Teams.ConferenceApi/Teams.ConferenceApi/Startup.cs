using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using Teams.ConferenceApi;
using Teams.ConferenceApi.Models;
using Teams.ConferenceApi.Repositories;
using Teams.ConferenceApi.Repositories.Interfaces;
using Teams.ConferenceApi.TokenManager;
using Teams.ConferenceApi.TokenManager.Extensions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Teams.ConferenceApi
{
    public class Startup
        : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddLogging();
            builder.Services.AddTokenValidatorDependencies(configuration);
            builder.Services.AddSingleton<IJsonTextSerializer, JsonTextSerializer>();

            builder.Services.Configure<StorageConfiguration>(tvc => configuration.Bind("StorageConfiguration", tvc));
            builder.Services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));

            builder.Services.AddSingleton<AbstractApplicationBuilder<ConfidentialClientApplicationBuilder>, ConfidentialClientApplicationBuilder>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<TokenCreatorConfiguration>>();
                var tokenCreatorConfiguration = options.Value;

                return ConfidentialClientApplicationBuilder
                    .Create(tokenCreatorConfiguration.ClientId)
                    .WithClientSecret(tokenCreatorConfiguration.ClientSecret)
                    .WithAuthority(AzureCloudInstance.AzurePublic, tokenCreatorConfiguration.TenantId);
            });

            builder.Services.AddSingleton<AbstractApplicationBuilder<PublicClientApplicationBuilder>, PublicClientApplicationBuilder>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<TokenCreatorConfiguration>>();
                var tokenCreatorConfiguration = options.Value;

                return PublicClientApplicationBuilder
                    .Create(tokenCreatorConfiguration.ClientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, tokenCreatorConfiguration.TenantId);
            });

            builder.Services.AddTokenCreatorDependencies(configuration);
            builder.Services.AddSingleton<IMicrosoftGraphRepository, MicrosoftGraphRepository>();
        }
    }
}
