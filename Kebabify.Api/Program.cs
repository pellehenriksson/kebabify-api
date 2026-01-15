using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;

using Kebabify.Api.Services;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();


        //// 1) Create a single DefaultAzureCredential and share it across all Azure clients
        //builder.Services.AddAzureClients(clientBuilder =>
        //{
        //    // Global credential: works locally (CLI/VS) and in Azure (Managed Identity)
        //    clientBuilder.UseCredential(new DefaultAzureCredential(
        //        new DefaultAzureCredentialOptions
        //        {
        //            // Optional dev-only tuning: e.g., disable Managed Identity if it causes noise locally
        //            // ExcludeManagedIdentityCredential = true
        //            // Retry = { MaxRetries = 5, ... } // You can also tune credential retry
        //        }));


        //    // 2) Register your BlobServiceClient (single storage account)
        //    // Prefer endpoint URI over connection strings when you use AAD auth.
        //    var storageEndpoint = new Uri(builder.Configuration["Storage:Endpoint"]!);
        //    clientBuilder.AddBlobServiceClient(storageEndpoint)
        //                 .ConfigureOptions((opts) =>
        //                 {
        //                     // Retry and timeouts tuned for interactive APIs
        //                     opts.Retry.Mode = RetryMode.Exponential;
        //                     opts.Retry.MaxRetries = 5;
        //                     opts.Retry.Delay = TimeSpan.FromSeconds(0.8);
        //                     opts.Retry.MaxDelay = TimeSpan.FromSeconds(30);
        //                     opts.Transport = new HttpClientTransport(new HttpClient
        //                     {
        //                         Timeout = TimeSpan.FromSeconds(100)
        //                     });
        //                 });
        //});


        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        //builder.Services.AddTransient<IStorageService, StorageService>();
        builder.Services.AddTransient<IKebabService, KebabService>();

        await builder.Build().RunAsync();
    }
}