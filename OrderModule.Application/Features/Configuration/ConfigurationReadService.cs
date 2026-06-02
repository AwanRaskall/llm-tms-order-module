using OrderModule.Application.Features.Configuration.Models;
using Raven.Client.Documents.Session;

namespace OrderModule.Application.Features.Configuration
{
    /// <summary>
    /// Reads LLM configuration from RavenDB.
    /// Creates and stores default configuration if none exists yet.
    /// </summary>
    public class ConfigurationReadService
    {
        private readonly IDocumentSession _session;

        public ConfigurationReadService(IDocumentSession session)
        {
            _session = session;
        }

        public ConfigurationModel GetConfiguration()
        {
            var config = _session.Load<ConfigurationModel>(ConfigurationModel.DefaultId);

            if (config == null)
            {
                config = new ConfigurationModel();
                _session.Store(config, ConfigurationModel.DefaultId);
                _session.SaveChanges();
            }

            return config;
        }
    }
}