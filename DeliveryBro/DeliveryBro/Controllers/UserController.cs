using DeliveryBro.Models;
using DeliveryBro.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DeliveryBro.Controllers
{   
    
    public class UserController : Controller
    {
        private readonly sql8005site4nownetContext _context;

        public UserController(sql8005site4nownetContext context) 
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
       
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult UserHome()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult UserOrder()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult UserOrderHistory()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult UserAddress()
        {
            return View();
        }
        

    }
}
