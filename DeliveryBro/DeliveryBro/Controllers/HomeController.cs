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

        //預設回傳View
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult RestaurantMenu()
        {

            return View();
        }

        public IActionResult OrderList()
        {
            return View();
        }
        //[Authorize]
        public IActionResult CheckoutList()
        {
            return View();
        }
        //[Authorize]
        public IActionResult PaybyCreditCard()
        {
            return View();
        }
        //[Authorize]
        public IActionResult FinalOrderList()
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