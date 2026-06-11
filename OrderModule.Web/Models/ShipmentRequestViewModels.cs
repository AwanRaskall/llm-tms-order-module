using System;

namespace OrderModule.Web.Models
{
    /// <summary>
    /// Views for Shipment Requests page
    /// </summary>
    public class ShipmentRequestsViewModel
    {
        // Filters
        public string Invoice { get; set; } = string.Empty;
        public string DepPoint { get; set; } = string.Empty;
        public string ArrPoint { get; set; } = string.Empty;
        public string Transport { get; set; } = string.Empty;
        public string CreatedOn { get; set; } = string.Empty;
        public string DepDate { get; set; } = string.Empty;
        public string ArrDate { get; set; } = string.Empty;

        // Table results 
        public ShipmentRequestItemViewModel[] Requests { get; set; } = Array.Empty<ShipmentRequestItemViewModel>();
    }

    /// <summary>
    /// One row in the Shipment Requests table
    /// </summary>
    public class ShipmentRequestItemViewModel
    {
        public string Id { get; set; } = string.Empty; // for delete function
        public DateTime CreatedOn { get; set; }
        public string Invoice { get; set; } = string.Empty;
        public string DepPoint { get; set; } = string.Empty;
        public string DepDate { get; set; } = string.Empty;
        public string ArrPoint { get; set; } = string.Empty;
        public string ArrDate { get; set; } = string.Empty;
        public string Transport { get; set; } = string.Empty;
        public string Products { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}