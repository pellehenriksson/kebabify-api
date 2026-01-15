using Kebabify.Api.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace Kebabify.Api
{
    public class Endpoints
    {
        private readonly IKebabService kebabService;
        
        private readonly ILogger<Endpoints> logger;

        public Endpoints(IKebabService kebabService, ILogger<Endpoints> logger)
        {
            this.kebabService = kebabService;
            this.logger = logger;
        }

        [Function("MakeKebab")]
        public async Task<IActionResult> MakeKebab([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "kebab")] HttpRequestData req)
        {
            try
            {
                this.logger.LogInformation("Try to make kebab");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<KebabRequest>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

                if (data == null)
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }   
                
                var result = this.kebabService.Create(data.Input);

                return new OkObjectResult(new KebabRespone(data.Input, result));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to make kebab");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }

    public record KebabRequest(string Input);

    public record KebabRespone(string Input, string Result);
}
