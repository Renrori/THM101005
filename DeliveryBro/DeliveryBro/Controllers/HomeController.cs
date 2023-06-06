using DeliveryBro.Models;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace DeliveryBro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly sql8005site4nownetContext _context;

        public HomeController(ILogger<HomeController> logger , sql8005site4nownetContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Authorize,AllowAnonymous]
        //預設回傳View
        public IActionResult Index()
        {
            return View();
        }
        [Authorize, AllowAnonymous]
        public IActionResult RestaurantMenu()
        {

            return View();
        }
        [Authorize, AllowAnonymous]
        public IActionResult OrderList()
        {
            return View();
        }
        [Authorize, AllowAnonymous]
        public IActionResult OrderListNav()
        {
            return View();
        }
        [Authorize(Roles = "User",AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult CheckoutList()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult PaybyCreditCard()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult FinalOrderList()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult FinalOrderListPaydone()
        {
            return View();
        }

        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult FinalOrderListPayfail()
        {
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}