using OrderModule.Application.Features.ShipmentRequests.Models;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace OrderModule.Application.Features.ShipmentRequests
{
    /// <summary>
    /// Reads ShipmentRequest documents from RavenDB with optional filtering.
    /// Returns domain models
    /// </summary>
    public class ShipmentRequestReadService
    {
        private readonly IDocumentSession _session;

        public ShipmentRequestReadService(IDocumentSession session)
        {
            _session = session;
        }

        public ShipmentRequest[] GetFiltered(
            string invoice,
            string depPoint,
            string arrPoint,
            string transport,
            string createdOn,
            string depDate,
            string arrDate)
        {
            var all = _session.Query<ShipmentRequest>()
                              .OrderByDescending(x => x.CreatedOn)
                              .ToList();

            var filtered = all.AsEnumerable();

            if (!string.IsNullOrEmpty(invoice))
                filtered = filtered.Where(r => r.Invoice.Contains(invoice, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(depPoint))
                filtered = filtered.Where(r => r.DepPoint.Contains(depPoint, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(arrPoint))
                filtered = filtered.Where(r => r.ArrPoint.Contains(arrPoint, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(transport))
                filtered = filtered.Where(r => r.Transport.Contains(transport, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(createdOn) &&
                DateTime.TryParse(createdOn, out var createdOnDate))
                filtered = filtered.Where(r => r.CreatedOn.Date == createdOnDate.Date);

            if (!string.IsNullOrEmpty(depDate))
                filtered = filtered.Where(r => r.DepDate == depDate);

            if (!string.IsNullOrEmpty(arrDate))
                filtered = filtered.Where(r => r.ArrDate == arrDate);

            return filtered.ToArray();
        }
    }
}