using Kebabify.Api.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Kebabify.Api
{
    public class Endpoints
    {
        private readonly IKebabService kebabService;

        private readonly IStorageService storageService;

        private readonly ILogger<Endpoints> logger;

        public Endpoints(IKebabService kebabService, IStorageService storageService, ILogger<Endpoints> logger)
        {
            this.kebabService = kebabService;
            this.storageService = storageService;
            this.logger = logger;
        }

        [Function("MakeKebab")]
        public async Task<IActionResult> MakeKebab([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "kebab")] HttpRequestData req)
        {
            try
            {
                this.logger.LogInformation("Processing kebab request");

                const long MaxBodySize = 1024;
                if (req.Body.Length > MaxBodySize)
                {
                    return new BadRequestObjectResult("Request body too large");
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<KebabRequest>(requestBody, options);

                if (data == null || string.IsNullOrWhiteSpace(data.Input))
                {
                    return new BadRequestObjectResult("Invalid JSON or empty input");
                }

                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(data);
                if (!Validator.TryValidateObject(data, context, validationResults, true))
                {
                    return new BadRequestObjectResult(validationResults.Select(r => r.ErrorMessage));
                }

                this.logger.LogInformation("Making the kebab");
                var result = this.kebabService.Create(data.Input);

                this.logger.LogInformation("Peristing result");
                await this.storageService.Persist(data.Input, result);

                this.logger.LogInformation("Returning result");
                return new OkObjectResult(new KebabRespone(data.Input, result));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to make kebab");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class KebabRequest
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(512, MinimumLength = 2)]
        public string Input { get; set; } = string.Empty;
    }

    public record KebabRespone(string Input, string Result);
}
