using System;
using System.IO;
using System.Threading.Tasks;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.Application.DomainServices.EmailExtraction.Services;
using OrderModule.Application.ExternalServices.OpenRouter;
using OrderModule.Application.Features.Configuration;

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
        private readonly ConfigurationReadService _configReadService;

        public MessageProcessor(
            OpenRouterService openRouterService,
            ConfigurationReadService configReadService)
        {
            _openRouterService = openRouterService;
            _configReadService = configReadService;
        }

        public async Task<ExtractedSummary> ProcessMessageAsync(Stream fileStream, string fileName)
        {
            string extractedText = ExtractTextService.ExtractText(fileStream, fileName);

            string prompt = PromptBuilder.BuildExtractionPrompt(extractedText);
            
            // the model will be taken from Configuration page
            string modelName = _configReadService.GetConfiguration().SelectedModel;
            
            string model;
            switch (modelName)
            {
                case "openrouter-free":
                    model = "openrouter/free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);
                
                case "gpt-oss-120b":
                    model = "openai/gpt-oss-120b:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                case "gpt-oss-20b":
                    model = "openai/gpt-oss-20b:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                case "gemma-4-26b-a4b-it":
                    model = "google/gemma-4-26b-a4b-it:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                case "nemotron3":
                    model = "nvidia/nemotron-3-super-120b-a12b:free";
                    return await _openRouterService.ExtractDataFromText(model, prompt);

                default:
                    throw new NotSupportedException("Model is not supported");
            }
        }
    }
}
