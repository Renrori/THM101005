using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Areas.admin.Controllers
{
    public class CustomerReviewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
