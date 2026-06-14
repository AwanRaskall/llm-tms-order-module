using Microsoft.AspNetCore.Mvc;
using OrderModule.Web.Models;
using OrderModule.Application.Features.OrderExtractorService;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.Application.Features.ShipmentRequests;

namespace OrderModule.Web.Controllers
{
    public class OrderExtractorController : Controller
    {
        
        private readonly MessageProcessor _messageProcessor;
        private readonly Normalizer _normalizer;
        private readonly ShipmentRequestService       _shipmentRequestService;
        private readonly ShipmentRequestReadService   _shipmentRequestReadService;
        
        public OrderExtractorController(
            MessageProcessor messageProcessor,
            Normalizer normalizer,
            ShipmentRequestService shipmentRequestService,
            ShipmentRequestReadService shipmentRequestReadService
        )
        {
            _messageProcessor = messageProcessor;
            _normalizer = normalizer;
            _shipmentRequestService = shipmentRequestService;
            _shipmentRequestReadService = shipmentRequestReadService;
        }
        
        public IActionResult Index()
        {
            return View(new ViewModelSummary());
        }


        // Working with Summary form
        [HttpPost]
        public async Task<IActionResult> ParseSummary(IFormFile file)
        {
            if (file == null)
                return BadRequest("File wasn't provided");

            using var stream = file.OpenReadStream();
            ExtractedSummary summary = await _messageProcessor.ProcessMessageAsync(stream, file.FileName);

            string DepDate = _normalizer.ConvertDate(summary.DepDate);
            string ArrDate = _normalizer.ConvertDate(summary.ArrDate);

            ViewModelSummary viewModel = new ViewModelSummary()
            {
                Invoice = summary.Invoice,
                DepDate = DepDate,
                DepPoint = summary.DepPoint,
                ArrDate = ArrDate,
                ArrPoint = summary.ArrPoint,
                Transport = summary.Transport,
                Products = summary.Products,
                Notes = summary.Notes,
            };
            return Ok(viewModel);
        }

        // Saves the record and goes to Shipment Requests page
        [HttpPost]
        public IActionResult CreateDraft(ViewModelSummary model)
        {
            _shipmentRequestService.Handle(
                model.Invoice,
                model.DepDate,
                model.DepPoint,
                model.ArrDate,
                model.ArrPoint,
                model.Transport,
                model.Products,
                model.Notes);

            return Ok(new { success = true });
        }


        // Filters come from the URL (?invoice=...&depPoint=...)
        [HttpGet]
        public IActionResult ShipmentRequests(
            string invoice,
            string depPoint,
            string arrPoint,
            string transport,
            string createdOn,
            string depDate,
            string arrDate
        )
        {
            // Application returns domain models
            var results = _shipmentRequestReadService.GetFiltered(
                invoice,
                depPoint,
                arrPoint,
                transport,
                createdOn,
                depDate,
                arrDate
            );

            // The web layer maps to its ViewModels
            var requests = results.Select(r => new ShipmentRequestItemViewModel
            {
                Id = r.Id,
                CreatedOn = r.CreatedOn,
                Invoice = r.Invoice,
                DepPoint = r.DepPoint,
                DepDate = r.DepDate,
                ArrPoint = r.ArrPoint,
                ArrDate = r.ArrDate,
                Transport = r.Transport,
                Products = r.Products,
                Notes = r.Notes
            }).ToArray();

            return View(new ShipmentRequestsViewModel
            {
                Requests = requests,
                Invoice = invoice ?? string.Empty,
                DepPoint = depPoint ?? string.Empty,
                ArrPoint = arrPoint ?? string.Empty,
                Transport = transport ?? string.Empty,
                CreatedOn = createdOn ?? string.Empty,
                DepDate = depDate ?? string.Empty,
                ArrDate = arrDate ?? string.Empty
            });
        }


        // id comes from asp-route-id in the delete form
        [HttpPost]
        public IActionResult DeleteShipmentRequest(string id)
        {
            _shipmentRequestService.Delete(id);
            return RedirectToAction("ShipmentRequests");
        }

    }
}