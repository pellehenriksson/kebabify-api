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
                clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("Storage"));
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