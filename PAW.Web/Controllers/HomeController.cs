using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(IExchangeRateService exchangeRateService, ILogger<HomeController> logger)
            : base(exchangeRateService)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                TipoCambio = base.TipoCambio
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Privacy()
        {
            var viewModel = new HomeViewModel
            {
                TipoCambio = base.TipoCambio
            };
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
