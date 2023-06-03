using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrator", AuthenticationSchemes = "AdministratorAuthenticationScheme")]
    public class HomeController : Controller
    {
        // [Authorize(AuthenticationSchemes = "admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChatRoom()
        {
            return View();
        }
    }
}
