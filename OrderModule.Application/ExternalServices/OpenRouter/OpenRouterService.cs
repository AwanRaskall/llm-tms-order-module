using Microsoft.Extensions.Configuration;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.Application.Interfaces; 
using System;
using System.Text.Json;
using System.Threading.Tasks;


namespace OrderModule.Application.ExternalServices.OpenRouter
{
    /// <summary>
    /// Sends extraction prompts to OpenRouter API.
    /// Uses the OpenAI SDK since OpenRouter follows the same API format.
    /// </summary>
    public class OpenRouterService: ILLMService
    {
        private readonly IHttpServiceOpenRouter _httpService;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OpenRouterService(IHttpServiceOpenRouter httpService)
        {
            _httpService = httpService;
        }

        /// <summary>
        /// Builds request, sends to OpenRouter and returns extracted order fields.
        /// </summary>
        public async Task<ExtractedSummary> ExtractDataFromText(
            string model,
            string prompt,
            float temperature = 0.5f,
            int max_tokens = 300)
        {
            var request = new OpenRouterRequest
            {
                Model       = model,
                Temperature = temperature,
                MaxTokens   = max_tokens,
                Messages    = new List<OpenRouterMessage>
                {
                    new() { Role = "system",
                            Content = "Return ONLY valid JSON. No markdown. No explanations." },
                    new() { Role = "user", Content = prompt }
                }
            };

            var response = await _httpService.SendAsync<OpenRouterRequest, OpenRouterResponse>("chat/completions", request);

            if (response == null)
                throw new InvalidOperationException("OpenRouter returned null response after all retry attempts.");


            var textContent   = response?.GetContent() ?? string.Empty;

            // Console.WriteLine("-- GetContent() result --");
            // Console.WriteLine(textContent);
            // Console.WriteLine("-- End content --");
            
            var extractedJson = Normalizer.ExtractJson(textContent);
            var result = JsonSerializer.Deserialize<ExtractedSummary>(extractedJson, JsonOptions);

            if (result == null)
                throw new InvalidOperationException("LLM returned a response that could not be parsed into order fields.");

            return result;
        }
    }
}