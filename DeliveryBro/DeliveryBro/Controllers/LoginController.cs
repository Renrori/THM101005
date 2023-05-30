using DeliveryBro.Models;
using DeliveryBro.ViewModels.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Facebook;

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
        public async Task<IActionResult> Index([Bind("UserAccount, UserPassword")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                //比對是否存在此ID用戶
                var checkId = _context.CustomersTable.FirstOrDefault(x => x.CustomerAccount == login.UserAccount );
                if (checkId == null)
                {
                    ViewBag.ErrorMessage = "找不到此帳號";
                    return View(login);                    
                }
                var checkPw = _context.CustomersTable.FirstOrDefault(x=>x.CustomerAccount == login.UserAccount && x.CustomerPassword ==login.UserPassword);
                 if (checkPw == null)
                {
                    ViewBag.ErrorMessage = "密碼錯誤";
                    return View(login);
                }
                
                var claims = new List<Claim>() {
                     new Claim(ClaimTypes.Name, checkPw.CustomerName),
                     new Claim (ClaimTypes.Role,"User"),
                     new Claim("CustomerId",checkPw.CustomerId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //ClaimsPrincipal也可以List
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = "輸入的內容有誤";
            return View(login);
        }

        public IActionResult FacebookLogin()
        {
            //prop

            var prop = new AuthenticationProperties
            {
                RedirectUri =Url.Action("FacebookRes")

            };
            //發請求內容並傳入
            return Challenge(prop,FacebookDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> FacebookRes()
        {
            //非同步去呼叫
            var result =await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
            //如果驗證成功
            if(result.Succeeded)
            {
                var claims = result.Principal.Claims.Select(x => new
                {
                    //打印Claims物件
                    x.Type,
                    x.Value,
                });
                return Json(claims);
            }
            return Ok();
        }
        public IActionResult SignUp()
        {
            return View();

        }
    }
}
