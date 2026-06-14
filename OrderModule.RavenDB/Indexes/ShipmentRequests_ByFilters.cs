using OrderModule.Application.Features.ShipmentRequests.Models;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace OrderModule.RavenDB.Indexes
{
    /// <summary>
    /// Static RavenDB index for ShipmentRequests collection.
    /// Enables efficient filtering and full-text search across all filter fields.
    /// Deployed to RavenDB at application startup via RavenDbStore.
    /// </summary>
    public class ShipmentRequests_ByFilters : AbstractIndexCreationTask<ShipmentRequest>
    {
        public ShipmentRequests_ByFilters()
        {
            Map = docs => from doc in docs
                          select new
                          {
                              doc.Invoice,
                              doc.DepPoint,
                              doc.ArrPoint,
                              doc.Transport,
                              doc.CreatedOn,
                              doc.DepDate,
                              doc.ArrDate
                          };

            // Full-text search - allows to filter by part of a word
            Index(x => x.Invoice, FieldIndexing.Search);
            Index(x => x.DepPoint, FieldIndexing.Search);
            Index(x => x.ArrPoint, FieldIndexing.Search);
            Index(x => x.Transport, FieldIndexing.Search);
        }
    }
}