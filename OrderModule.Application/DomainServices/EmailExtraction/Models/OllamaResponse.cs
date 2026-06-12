using OrderModule.Application.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace OrderModule.Application.DomainServices.EmailExtraction.Models
{
    /// <summary>
    /// Raw response from Ollama /api/generate endpoint.
    /// Example response structure:
    /// { "model": "mistral:7b", "created_at": "...", "response": "{...json...}", "done": true }
    /// </summary>
    public class OllamaResponse : IModelResponse
    {
        public string Model { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string GetContent() => Response;
    }
}