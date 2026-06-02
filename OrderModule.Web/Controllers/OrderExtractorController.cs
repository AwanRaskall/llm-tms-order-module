using Microsoft.AspNetCore.Mvc;
using OrderModule.Web.Models;
using OrderModule.Application.Features.OrderExtractorService;
using OrderModule.Application.Features.OrderExtractorService.Models;
using OrderModule.Application.Features.OrderExtractorService.Utils;

namespace OrderModule.Web.Controllers
{
    public class OrderExtractorController : Controller
    {
        
        private readonly MessageProcessor _messageProcessor;
        private readonly Normalizer _normalizer;
        
        public OrderExtractorController(
            MessageProcessor messageProcessor,
            Normalizer normalizer
        )
        {
            _messageProcessor = messageProcessor;
            _normalizer = normalizer;
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

        [HttpPost]
        public virtual async Task<IActionResult> CreateDraft(ViewModelSummary model)
        {
            return RedirectToAction("Drafts");
        }

    }
}