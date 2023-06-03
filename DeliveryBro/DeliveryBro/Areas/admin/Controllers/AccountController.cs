using DeliveryBro.ViewModels.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DeliveryBro.Models;
using DeliveryBro.Areas.admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace DeliveryBro.Areas.admin.Controllers
{
    [Area("admin")]
    public class AccountController : Controller
    {
        // DB連線CONTEXT
        private readonly sql8005site4nownetContext _db;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="context"></param>
        public AccountController(sql8005site4nownetContext context)
        {
            this._db = context;
        }

        // 登入頁面      
        //[HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 送出登入
        [HttpPost]
        public async Task<IActionResult> Login(AdministerUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 讀取表單資料
                var acc = model.AdministerAccount;
                var pwd = model.AdministerPassword;

                // 檢查資料庫確認帳號密碼
                var user = _db.AdministerTable.FirstOrDefault(x =>
                x.AdministerAccount == acc &&
                x.AdministerPassword == pwd);

                // 如果登入失敗
                if (user == null)
                {
                    // 顯示錯誤
                    ViewBag.Error = "帳號密碼錯誤";
                    return View("login");
                }

                // 使用者參數，權限
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.AdministerAccount),
                    new Claim(ClaimTypes.Role, "Administrator"),
                    new Claim("AdministratorId",user.AdministerId.ToString())
                };

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };
                var claimsIdentity = new ClaimsIdentity(claims, "AdministratorAuthenticationScheme");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("AdministratorAuthenticationScheme", claimsPrincipal);
                //await HttpContext.SignInAsync(
                //        CookieAuthenticationDefaults.AuthenticationScheme,
                //        new ClaimsPrincipal(claimsIdentity),
                //    authProperties);

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "輸入的內容有誤";
            return View("Login");
        }

        // 登出
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdministratorAuthenticationScheme");
            return RedirectToAction("Index", "Home");
        }
    }
}
