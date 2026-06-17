using System.Collections.Generic;

namespace OrderModule.Application.ExternalServices.OpenRouter
{
    /// <summary>
    /// Request body sent to OpenRouter /v1/chat/completions endpoint.
    /// </summary>
    public class OpenRouterRequest
    {
        public string Model { get; set; } = string.Empty;
        public List<OpenRouterMessage> Messages { get; set; } = new();
        public float Temperature { get; set; } = 0.5f;
        public int MaxTokens { get; set; } = 300;
    }

    public class OpenRouterMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}