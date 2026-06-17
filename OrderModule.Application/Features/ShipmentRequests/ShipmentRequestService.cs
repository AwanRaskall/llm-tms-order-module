using OrderModule.Application.Features.ShipmentRequests.Models;
using Raven.Client.Documents.Session;

namespace OrderModule.Application.Features.ShipmentRequests
{
    /// <summary>
    /// Creates and deletes shipment requests in RavenDB.
    /// </summary>
    public class ShipmentRequestService
    {
        private readonly IDocumentSession _session;

        public ShipmentRequestService(IDocumentSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Creates a new ShipmentRequest document in RavenDB.
        /// </summary>
        public void Handle(
            string invoice,
            string depDate,
            string depPoint,
            string arrDate,
            string arrPoint,
            string transport,
            string products,
            string notes)
        {
            var request = new ShipmentRequest(
                invoice?.Trim() ?? string.Empty,
                depDate?.Trim() ?? string.Empty,
                depPoint?.Trim() ?? string.Empty,
                arrDate?.Trim() ?? string.Empty,
                arrPoint?.Trim() ?? string.Empty,
                transport?.Trim() ?? string.Empty,
                products?.Trim() ?? string.Empty,
                notes?.Trim() ?? string.Empty);

            _session.Store(request);
            _session.SaveChanges();
        }

        /// <summary>
        /// Deletes a ShipmentRequest document by ID.
        /// UI deletes one at a time.
        /// </summary>
        public void Delete(string id)
        {
            _session.Delete(id);
            _session.SaveChanges();
        }
    }
}