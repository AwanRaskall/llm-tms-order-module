using System.Collections.Generic;
using System.Linq;

namespace OrderModule.Application.ExternalServices.OpenRouter
{
    /// <summary>
    /// Represents the raw response structure from OpenRouter API.
    /// Example response structure:
    ///     "choices": [{ "message": { "role": "assistant", "content": "..." } }]
    /// </summary>
    public class OpenRouterResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<Choice> Choices { get; set; }

        public OpenRouterResponse()
        {
            Choices = new List<Choice>
            {
                new Choice
                {
                    Index = 0,
                    Message = new ResponseMessage
                    {
                        Role = "assistant",
                        Content = string.Empty
                    },
                    FinishReason = null
                }
            };
        }

        public string GetContent() => Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
    }

    public class Choice
    {
        public int Index { get; set; }
        public ResponseMessage Message { get; set; } = new();
        public string? FinishReason { get; set; }
    }

    public class ResponseMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}