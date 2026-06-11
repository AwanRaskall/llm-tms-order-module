using System;

namespace OrderModule.Application.Features.ShipmentRequests.Models
{
    /// <summary>
    /// RavenDB document representing a saved shipment request.
    /// All fields are plain strings from LLM output.
    /// </summary>
    public sealed class ShipmentRequest
    {
        public string Id { get; private set; } = string.Empty;
        public DateTime CreatedOn { get; private set; }
        public string Invoice { get; private set; } = string.Empty;
        public string DepDate { get; private set; } = string.Empty;
        public string DepPoint { get; private set; } = string.Empty;
        public string ArrDate { get; private set; } = string.Empty;
        public string ArrPoint { get; private set; } = string.Empty;
        public string Transport { get; private set; } = string.Empty;
        public string Products { get; private set; } = string.Empty;
        public string Notes { get; private set; } = string.Empty;

        // RavenDB requires a parameterized constructor for deserialization
        private ShipmentRequest() { }

        /// <summary>
        /// Creates a new shipment request from LLM-extracted fields.
        /// </summary>
        public ShipmentRequest(
            string invoice,
            string depDate,
            string depPoint,
            string arrDate,
            string arrPoint,
            string transport,
            string products,
            string notes)
        {
            CreatedOn = DateTime.UtcNow;
            Invoice = invoice ?? string.Empty;
            DepDate = depDate ?? string.Empty;
            DepPoint = depPoint ?? string.Empty;
            ArrDate = arrDate ?? string.Empty;
            ArrPoint = arrPoint ?? string.Empty;
            Transport = transport ?? string.Empty;
            Products = products ?? string.Empty;
            Notes = notes ?? string.Empty;
        }
    }
}