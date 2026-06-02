using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;

namespace OrderModule.RavenDB.Connection
{
    /// <summary>
    /// Creates and initializes the RavenDB DocumentStore.
    /// </summary>
    public static class RavenDbStore
    {
        private static IDocumentStore? _store;
        private static readonly object _lock = new();

        public static IDocumentStore Initialize(IConfiguration configuration)
        {
            if (_store != null)
                return _store;

            lock (_lock)
            {
                if (_store != null)
                    return _store;

                var urls     = configuration
                    .GetSection("RavenDB:Urls")
                    .Get<string[]>();
                var database = configuration["RavenDB:DatabaseName"];

                var store = new DocumentStore
                {
                    Urls = urls,
                    Database = database
                };

                var certPath = configuration["RavenDB:CertPath"];
                var certPassword = configuration["RavenDB:CertPassword"] ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(certPath))
                {
                    store.Certificate = new X509Certificate2(
                        certPath,
                        certPassword,
                        X509KeyStorageFlags.UserKeySet      |
                        X509KeyStorageFlags.PersistKeySet   |
                        X509KeyStorageFlags.Exportable
                    );
                }

                store.Initialize();
                _store = store;
            }
            return _store;
        }
    }
}