using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.admin.Controllers
{
    [Area("admin")]
    public class MemberController : Controller
    {
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View("List");
        }

        public IActionResult Serach()
        {
            return View("Serach");
        }
    }
}
