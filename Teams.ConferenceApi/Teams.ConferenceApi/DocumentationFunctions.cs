using Aliencube.AzureFunctions.Extensions.OpenApi;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Configurations;
using Aliencube.AzureFunctions.Extensions.OpenApi.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Teams.ConferenceApi
{
    public class DocumentationFunctions
    {
        private const string DocumentName = "openapi.json";
        private const string AuthenticationParameter = "code";

        [OpenApiIgnore]
        [FunctionName(nameof(GenerateJsonDocumentation))]
        public async Task<IActionResult> GenerateJsonDocumentation([HttpTrigger(AuthorizationLevel.Function, "get", Route = DocumentName)] HttpRequest req)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInformation = FileVersionInfo.GetVersionInfo(assembly.Location);
            var documentHelper = new DocumentHelper(new RouteConstraintFilter());
            var document = new Document(documentHelper);
            var result = await document.InitialiseDocument()
                .AddMetadata(new OpenApiInfo()
                {
                    Title = fileVersionInformation.ProductName,
                    Description = fileVersionInformation.Comments,
                    Contact = new OpenApiContact()
                    {
                        Name = fileVersionInformation.CompanyName
                    },
                    Version = fileVersionInformation.FileVersion,
                })
                .AddServer(req, "api")
                .Build(assembly)
                .RenderAsync(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);

            var response = new ContentResult()
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        [OpenApiIgnore]
        [FunctionName(nameof(GenerateDocumentationInterface))]
        public async Task<IActionResult> GenerateDocumentationInterface([HttpTrigger(AuthorizationLevel.Function, "get", Route = "docs")] HttpRequest req)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            var assembly = Assembly.GetExecutingAssembly();
            var authCode = req.Query[AuthenticationParameter];
            var fileVersionInformation = FileVersionInfo.GetVersionInfo(assembly.Location);
            var swaggerUi = new SwaggerUI();
            var result = await swaggerUi
                .AddMetadata(new OpenApiInfo()
                {
                    Title = fileVersionInformation.ProductName,
                    Description = fileVersionInformation.Comments,
                    Contact = new OpenApiContact()
                    {
                        Name = fileVersionInformation.CompanyName
                    },
                    Version = fileVersionInformation.FileVersion,
                })
                .AddServer(req, "api")
                .BuildAsync()
                .RenderAsync("openapi.json", authCode);

            var response = new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }
    }
}
