using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.admin.Controllers
{
    [Area("admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
