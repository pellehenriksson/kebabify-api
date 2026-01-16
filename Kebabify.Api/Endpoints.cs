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
    public class Endpoints(IKebabService kebabService, IStorageService storageService, ILogger<Endpoints> logger)
    {
        private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        [Function("MakeKebab")]
        public async Task<IActionResult> MakeKebab([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "kebab")] HttpRequestData req)
        {
            try
            {
                logger.LogInformation("Processing kebab request");

                const long MaxBodySize = 1024;
                if (req.Body.Length > MaxBodySize)
                {
                    return new BadRequestObjectResult("Request body over the limit");
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
             
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

                logger.LogInformation("Making the kebab");
                var result = kebabService.Create(data.Input);

                logger.LogInformation("Peristing result");
                await storageService.Persist(data.Input, result);

                logger.LogInformation("Returning result");
                return new OkObjectResult(new KebabRespone(data.Input, result));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to make kebab");

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
