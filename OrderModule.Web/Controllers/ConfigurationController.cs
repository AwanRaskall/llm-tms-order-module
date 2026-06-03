using Microsoft.AspNetCore.Mvc;
using OrderModule.Application.Features.Configuration;
using OrderModule.Web.Models;

namespace OrderModule.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ConfigurationReadService _readService;
        private readonly ConfigurationService _configService;

        public ConfigurationController(
            ConfigurationReadService readService,
            ConfigurationService configService)
        {
            _readService = readService;
            _configService = configService;
        }

        // GET /Configuration/Index
        public IActionResult Index()
        {
            var config = _readService.GetConfiguration();
            return View(new ConfigViewModel(config));
        }

        // POST /Configuration/Index
        [HttpPost]
        public IActionResult Index(ConfigViewModel model)
        {
            _configService.Handle(model.ToConfigurationModel());
            return RedirectToAction("Index");
        }
    }
}