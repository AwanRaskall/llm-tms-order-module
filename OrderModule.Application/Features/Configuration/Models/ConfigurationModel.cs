
namespace OrderModule.Application.Features.Configuration.Models
{
    /// <summary>
    /// Stores the user-selected LLM configuration.
    /// Persisted in RavenDB with a fixed document ID.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Fixed document ID in RavenDB
        /// </summary>
        public const string DefaultId = "OrderModuleConfiguration/1-A";

        public string Id { get; set; } = DefaultId;

        /// <summary>
        /// Key of the selected model ("openrouter-free", "deepseek", others)
        /// Maps to a case in MessageProcessor switch statement.
        /// </summary>
        public string SelectedModel { get; set; } = "openrouter-free";

        /// <summary>
        /// Selected service provider: "openrouter", "ollama", or empty string
        /// </summary>
        public string SelectedService { get; set; } = string.Empty;
    }
}