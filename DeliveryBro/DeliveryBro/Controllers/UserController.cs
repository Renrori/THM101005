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
        //public Task<IActionResult> Login(LoginViewModel lm)
        //{
        //    //比對是否存在此ID用戶
        //    var user = _context.CustomersTable.FirstOrDefault(x => x.CustomerId == lm.UserID && x.CustomerPassword == lm.UserPassword);
        //    if (user != null)
        //    { 


        //    }

        //    return View();
        //}
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult UserHome()
        {
            return View();
        }
        [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
        public IActionResult UserOrderHistory()
        {
            return View();
        }

    }
}
