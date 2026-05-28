using Microsoft.AspNetCore.Mvc;
using OrderModule.Web.Models;
using OrderModule.Application.OrderExtractorService;
using OrderModule.Application.OrderExtractorService.Models;
using OrderModule.Application.OrderExtractorService.Utils;

namespace OrderModule.Web.Controllers
{
    public class OrderExtractorController : Controller
    {
        
        private readonly MessageProcessor _messageProcessor;
        
        public OrderExtractorController(
            MessageProcessor messageProcessor
        )
        {
            _messageProcessor = messageProcessor;
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

            ViewModelSummary viewModel = new ViewModelSummary()
            {
                Invoice = summary.Invoice,
                DepDate = summary.DepDate,
                DepPoint = summary.DepPoint,
                ArrDate = summary.ArrDate,
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