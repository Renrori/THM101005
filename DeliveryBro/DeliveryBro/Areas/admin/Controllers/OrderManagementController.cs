using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.admin.Controllers
{
    public class OrderManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
