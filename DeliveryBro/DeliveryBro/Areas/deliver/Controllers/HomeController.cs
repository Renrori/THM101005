using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.deliver.Controllers
{
	[Area("deliver")]
	public class HomeController : Controller
	{
		public IActionResult Login()
		{
			return View();
		}
		public IActionResult Register()
		{
			return View();
		}
		public IActionResult DeliverAceptedOrder()
		{
			return View();
		}
		public IActionResult DeliverWaitingOrder()
		{
			return View();
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("DeliverAuthenticationScheme");
			return RedirectToAction("Login", "Home");
		}
		public IActionResult HistoryOrder()
		{
			return View();
		}
		public IActionResult LIFFPage()
		{
			return View();
		}
	}
}
