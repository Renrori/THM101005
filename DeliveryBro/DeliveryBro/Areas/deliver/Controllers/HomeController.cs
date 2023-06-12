using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("DeliverAuthenticationScheme");
			return RedirectToAction("Login", "Home");
		}
		[Authorize(Roles = "Deliver", AuthenticationSchemes = "DeliverAuthenticationScheme")]
		public IActionResult LIFFPage()
		{
			return View();
		}
	}
}
