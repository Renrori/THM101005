using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.store.Controllers
{
	[Area("store")]
	public class HomeController : Controller
	{
		
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
		{
			return View();
		}
		public IActionResult StoreInfo()
		{
			return View();
		}
		public IActionResult OrdersInTime()
		{
			return View();
		}
		public IActionResult HisOrder()
		{
			return View();
		}
		public IActionResult Menu()
		{
			return View();
		}
		public IActionResult HelpCenter()
		{
			return View();
		}
	}
}
