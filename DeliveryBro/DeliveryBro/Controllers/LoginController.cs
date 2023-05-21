using DeliveryBro.Models;
using DeliveryBro.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryBro.Controllers
{   
    
    public class LoginController : Controller
    {
        private readonly sql8005site4nownetContext _context;

        public LoginController(sql8005site4nownetContext context) 
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
        //    var loginTry = 0;
        //    if (user != null)
        //    {


        //    }

        //    return View();
        //}
    }
}
