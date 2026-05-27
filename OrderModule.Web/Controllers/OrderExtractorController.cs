using Microsoft.AspNetCore.Mvc;
using OrderModule.Web.Models;



namespace OrderModule.Web.Controllers
{
    public class OrderExtractorController : Controller
    {
        public IActionResult Index()
        {
            return View(new ViewModelSummary());
        }


        // Working with Summary form
        [HttpPost]
        public virtual async Task<IActionResult> ParseSummary(IFormFile file)
        {
            ViewModelSummary viewModel = new ViewModelSummary()
            {
                
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