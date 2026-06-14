using OrderModule.Application.Features.Configuration.Models;
using Raven.Client.Documents.Session;

namespace OrderModule.Application.Features.Configuration
{
    /// <summary>
    /// Saves updated LLM configuration to RavenDB.
    /// Creates new document if none exists, updates existing otherwise.
    /// </summary>
    public class ConfigurationService
    {
        private readonly IDocumentSession _session;

        public ConfigurationService(IDocumentSession session)
        {
            _session = session;
        }

        public void Handle(ConfigurationModel config)
        {
            var cfg = _session.Load<ConfigurationModel>(ConfigurationModel.DefaultId);

            if (cfg == null)
            {
                cfg = new ConfigurationModel();
                _session.Store(cfg, ConfigurationModel.DefaultId);
            }

            cfg.SelectedModel = config.SelectedModel;
            cfg.SelectedService = config.SelectedService;

            _session.SaveChanges();
        }
    }
}