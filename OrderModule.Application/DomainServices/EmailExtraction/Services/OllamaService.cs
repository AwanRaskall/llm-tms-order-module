using OrderModule.Application.DomainServices.EmailExtraction.Models;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.Application.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderModule.Application.DomainServices.EmailExtraction.Services
{
    /// <summary>
    /// Sends extraction prompts to a locally running Ollama instance
    /// Uses IHttpServiceOllama for HTTP communication.
    /// </summary>
    public class OllamaService : ILLMService
    {
        private readonly IHttpServiceOllama _httpService;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OllamaService(IHttpServiceOllama httpService)
        {
            _httpService = httpService;
        }

        public async Task<ExtractedSummary> ExtractDataFromText(
            string model,
            string prompt,
            float temperature = 0.5f,
            int max_tokens = 300)
        {
            var parameters = new
            {
                model,
                prompt,
                options = new
                {
                    temperature,
                    num_predict = max_tokens
                },
                stream = false
            };

            IModelResponse response;
            try
            {
                response = await _httpService.SendAsync<object, OllamaResponse>("/api/generate", parameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ollama request failed: {ex.Message}", ex);
            }

            string content = response.GetContent();
            string extractedJson = Normalizer.ExtractJson(content);

            // Parse json into ExtractedSummary
            return JsonSerializer.Deserialize<ExtractedSummary>(extractedJson, JsonOptions) ?? new ExtractedSummary();
        }
    }
}