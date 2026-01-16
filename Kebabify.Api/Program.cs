using Azure.Identity;

using Kebabify.Api.Services;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kebabify.Api
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = FunctionsApplication.CreateBuilder(args);

            builder.ConfigureFunctionsWebApplication();

            // Register BlobServiceClient using DI
            builder.Services.AddAzureClients(clientBuilder =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // LOCAL: Uses "Storage__ConnectionString" from local.settings.json
                    clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("Storage"));
                }
                else
                {
                    // PRODUCTION: Uses "Storage__serviceUri" and Managed Identity
                    clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("Storage"));
                    clientBuilder.UseCredential(new DefaultAzureCredential());
                }
            });

            builder.Services
                .AddApplicationInsightsTelemetryWorkerService()
                .ConfigureFunctionsApplicationInsights();

            builder.Services.AddTransient<IStorageService, StorageService>();
            builder.Services.AddTransient<IKebabService, KebabService>();

            await builder.Build().RunAsync();
        }
    }
}