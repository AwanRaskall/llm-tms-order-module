using OrderModule.Application.Features.Configuration.Models;

namespace OrderModule.Web.Models
{
    /// <summary>
    /// ViewModel for the Configuration page.
    /// Maps between ConfigurationModel (.Application) and the HTML form
    /// </summary>
    public class ConfigViewModel
    {
        public string SelectedModel { get; set; } = string.Empty;
        public string SelectedService { get; set; } = string.Empty;

        public ConfigViewModel() { }

        /// <summary>
        /// Builds ViewModel from domain model - used in GET controller action.
        /// </summary>
        public ConfigViewModel(ConfigurationModel config)
        {
            SelectedModel = config.SelectedModel;
            SelectedService = config.SelectedService;
        }

        /// <summary>
        /// Converts back to domain model - used in POST controller action.
        /// </summary>
        public ConfigurationModel ToConfigurationModel()
        {
            return new ConfigurationModel
            {
                SelectedModel = SelectedModel,
                SelectedService = SelectedService
            };
        }
    }
}