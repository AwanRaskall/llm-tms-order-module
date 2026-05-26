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
    }
}