using OrderModule.Application.Features.OrderExtractorService.Models;
using System.Threading.Tasks;

namespace OrderModule.Application.Interfaces
{
    /// <summary>
    /// Common contract for all LLM services (Ollama, OpenRouter).
    /// Allows MessageProcessor to work with any LLM without knowing which specific service is being used.
    /// </summary>
    public interface ILLMService
    {
        /// <summary>
        /// Sends a prompt to the LLM model and returns extracted order fields.
        /// </summary>
        /// <param name='model'>Name or identifier of the model to use.</param>
        /// <param name='prompt'>Prompt guiding the model in data extraction.</param>
        /// <param name='temperature'>Sampling temperature (default 0.5).</param>
        /// <param name='max_tokens'>Maximum tokens in the response (default 300).</param>
        /// <returns>ExtractedSummary with the 8 extracted order fields.</returns>
        Task<ExtractedSummary> ExtractDataFromText(
            string model,
            string prompt,
            float temperature = 0.5f,
            int max_tokens = 300
        );
    }
}