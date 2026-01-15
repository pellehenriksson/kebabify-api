using Microsoft.Extensions.Logging;

using System.Text;

namespace Kebabify.Api.Services
{
    public class KebabService(ILogger<KebabService> logger) : IKebabService
    {
        public string Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                logger.LogWarning("{Input} is null", nameof(input));
                return string.Empty;
            }

            //// split words
            logger.LogDebug("Split '{Input}' into words", input);
            var words = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            //// clean
            for (var index = 0; index < words.Length; index++)
            {
                logger.LogDebug("Clean: '{Word}'", words[index]);
                words[index] = RemoveSpecialCharacters(words[index]);
            }

            //// if no word left
            if (words.All(string.IsNullOrWhiteSpace))
            {
                logger.LogWarning("There are no words left after cleaning");
                return string.Empty;
            }

            //// join words
            logger.LogDebug("Join words and lower case");
            var result = words.Where(x => !string.IsNullOrWhiteSpace(x)).Aggregate((current, aggregated) => $"{current}-{aggregated}").ToLowerInvariant();

            logger.LogDebug("Result is: '{Result}'", result);

            return result;
        }

        private static string RemoveSpecialCharacters(string word)
        {
            var sb = new StringBuilder();
            foreach (var c in from c in word
                              where char.IsLetterOrDigit(c)
                              select c)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
