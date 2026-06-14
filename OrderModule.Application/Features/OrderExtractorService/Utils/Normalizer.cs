using System;
using System.Globalization;
using System.Text.Json;

namespace OrderModule.Application.Features.OrderExtractorService.Utils
{
    public class Normalizer
    {
        /// <summary>
        /// Converts a date string returned by LLM into 'yyyy-MM-dd' format required by HTML input type='date'.
        /// Returns empty string if the date cannot be parsed.
        /// </summary>
        public string ConvertDate(string input)
        {
            string convertedDate = string.Empty;
            string[] formats =
            {
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "dd.MM.yyyy",
                "MMMM d, yyyy",
                "MMM d, yyyy",
                "yyyy-MM-dd",
                "dd MMMM",
                "dd-MMMM",
                "MMMM dd",
                "MMMM-dd",

            };

            if (DateTime.TryParseExact(
                    input,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                convertedDate = date.ToString("yyyy-MM-dd");
                return convertedDate;
            }
            return convertedDate;
        }

        /// <summary>
        /// Extracts a JSON object from raw LLM response content.
        /// Handles three cases: markdown code block (```json...```), clean JSON, and JSON embedded within surrounding text
        /// </summary>
        public static string ExtractJson(string content)
        {
            try
            {
                content = content.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Normalizer: ExtractJson: {ex.Message}");
                throw;
            }

            // Case 1: markdown code block  ```json ... ```
            if (content.StartsWith("```"))
            {
                int firstBrace = content.IndexOf('{');
                int lastBrace = content.LastIndexOf('}');

                if (firstBrace >= 0 && lastBrace >= 0 && lastBrace > firstBrace)
                {
                    return content.Substring(firstBrace, lastBrace - firstBrace + 1);
                }
            }

            // Case 2: clean Json object
            if (content.StartsWith('{') && content.EndsWith('}'))
            {
                return content;
            }

            // Case 3: Json embedded between other text
            int start = content.IndexOf('{');
            int end = content.LastIndexOf('}');

            if (start >= 0 && end >= 0 && end > start)
            {
                return content.Substring(start, end - start + 1);
            }

            throw new JsonException("Unable to extract JSON from model response.");
        }

    }
}
