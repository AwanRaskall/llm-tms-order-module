using System;
using System.IO;
using System.Threading.Tasks;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.Application.DomainServices.EmailExtraction.Services;
using OrderModule.Application.ExternalServices.OpenRouter;


namespace OrderModule.Application.Features.OrderExtractorService
{
    /// <summary>
    /// Main orchestrator that connects all backend services.
    /// Extracts text from email file, selects the configured LLM,
    /// runs extraction, normalizes dates and returns structured order fields.
    /// </summary>
    public class MessageProcessor
    {
        private readonly OpenRouterService _openRouterService;

        public MessageProcessor(OpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;
        }

        public async Task<ExtractedSummary> ProcessMessageAsync(Stream fileStream, string fileName)
        {
            string extractedText = ExtractTextService.ExtractText(fileStream, fileName);

            string prompt = PromptBuilder.BuildExtractionPrompt(extractedText);
            
            // the model will be taken from Configuration page
            string modelName = "openrouter-free";
            
            string model;
            switch (modelName)
            {
                case "openrouter-free":
                    model = "openrouter/free";            // ← автовыбор лучшей бесплатной
                    return await _openRouterService.ExtractDataFromText(model, prompt);
                
                case "llama4-maverick":
                    model = "meta-llama/llama-4-maverick:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                case "llama4-scout":
                    model = "meta-llama/llama-4-scout:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                case "deepseek":
                    model = "deepseek/deepseek-r1:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                case "nemotron":
                    model = "nvidia/nemotron-3-super-120b-a12b:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                default:
                    throw new NotSupportedException("Model is not supported");
            }
        }
    }
}
