using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.Application.Interfaces; 
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.ClientModel;


namespace OrderModule.Application.ExternalServices.OpenRouter
{
    /// <summary>
    /// Sends extraction prompts to OpenRouter API.
    /// Uses the OpenAI SDK since OpenRouter follows the same API format.
    /// </summary>
    public class OpenRouterService: ILLMService
    {
        private OpenAIClient? _client;
        private readonly IConfiguration _configuration;

        public OpenRouterService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private OpenAIClient GetClient()
        {
            if (_client != null)
                return _client;

            var apiKey = _configuration["OpenRouter:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenRouter:ApiKey is missing in configuration");


            // Specify the OpenRouter address instead of the default OpenAI address
            _client = new OpenAIClient(
                new ApiKeyCredential(apiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1")
                }
            );

            return _client;
        }

        /// <summary>
        /// Sends a prompt to the specified OpenRouter model and returns the extracted order data as ExtractedSummary.
        /// </summary>
        public async Task<ExtractedSummary> ExtractDataFromText(
            string model,
            string prompt,
            float temperature = 0.5f,
            int max_tokens = 300)
        {
            var client = GetClient();
            var chatClient = client.GetChatClient(model);

            ChatMessage[] messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(
                    "Return ONLY valid JSON. No markdown. No explanations."
                ),
                ChatMessage.CreateUserMessage(prompt)
            };

            try
            {
                var completion = await chatClient.CompleteChatAsync(
                    messages,
                    new ChatCompletionOptions
                    {
                        Temperature = temperature
                    }
                );

                string content = completion.Value.Content[0].Text;
                string json = Normalizer.ExtractJson(content);

                var result = JsonSerializer.Deserialize<ExtractedSummary>(json);
                if (result == null)
                    throw new InvalidOperationException("LLM returned a response that could not be parsed into order fields");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}