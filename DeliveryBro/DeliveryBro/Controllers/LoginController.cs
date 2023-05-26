using DeliveryBro.Models;
using DeliveryBro.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DeliveryBro.Controllers
{

    public class LoginController : Controller
    {
        private readonly sql8005site4nownetContext _context;

        public LoginController(sql8005site4nownetContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([Bind("UserAccount, UserPassword")]LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                //比對是否存在此ID用戶
                var user = _context.CustomersTable.FirstOrDefault(x => x.CustomerAccount == login.UserAccount && x.CustomerPassword == login.UserPassword);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "找不到此帳號!";
                    return View(login);
                }

            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult SignUp()
        {
            return View();

        }
    }
}
