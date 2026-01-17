using Kebabify.Api.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Kebabify.Api
{
    public class Endpoints(IKebabService kebabService, IStorageService storageService, ILogger<Endpoints> logger)
    {
        private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        [Function("MakeKebab")]
        [OpenApiOperation(operationId: "MakeKebab", tags: ["Kebab"], Summary = "Convert string to kebab-case", Description = "Converts an input string to kebab-case format and persists the result to storage.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(KebabRequest), Required = true, Description = "The input string to convert to kebab-case")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(KebabRespone), Summary = "Successful response", Description = "Returns the original input and the kebab-case result")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Summary = "Bad request", Description = "Invalid input or validation error")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Summary = "Server error", Description = "An unexpected error occurred")]
        public async Task<IActionResult> MakeKebab([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "kebab")] HttpRequestData req)
        {
            try
            {
                logger.LogInformation("Processing kebab request");

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
        [OpenApiProperty(Description = "The input string to convert to kebab-case (2-512 characters)")]
        public string Input { get; set; } = string.Empty;
    }

    public record KebabRespone(
        [property: OpenApiProperty(Description = "The original input string")] string Input,
        [property: OpenApiProperty(Description = "The kebab-case result")] string Result);
}
