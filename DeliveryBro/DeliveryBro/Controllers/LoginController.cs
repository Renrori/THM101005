using DeliveryBro.Models;
using DeliveryBro.ViewModels.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using DeliveryBro.Services;

namespace DeliveryBro.Controllers
{

    public class LoginController : Controller
    {
        private readonly sql8005site4nownetContext _context;
        private readonly PasswordEncyptService _passwordEncyptService;

        public LoginController(sql8005site4nownetContext context,PasswordEncyptService passwordEncyptService)
        {
            _context = context;
            _passwordEncyptService= passwordEncyptService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("UserAccount, UserPassword")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var encyptpassword = _passwordEncyptService.PasswordEncrypt(login.UserPassword);
                //比對是否存在此ID用戶
                var user = _context.CustomersTable.FirstOrDefault(x => x.CustomerAccount == login.UserAccount && x.CustomerPassword == encyptpassword);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "找不到此帳號";
                    return View(login);
                }
                
                var claims = new List<Claim>() {
                     new Claim(ClaimTypes.Name, user.CustomerName),
                     new Claim (ClaimTypes.Role,"User"),
                     new Claim("CustomerId",user.CustomerId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = "輸入的內容有誤";
            return View(login);
        }
        public IActionResult Register()
        {
            return View();
        }

	}
}
