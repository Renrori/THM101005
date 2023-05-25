using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Areas.store.Services;
using DeliveryBro.Areas.store.ViewModels;
using DeliveryBro.Models;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DeliveryBro.Areas.store.Controllers
{
    [Area("store")]
	public class UserController : Controller
    {
        private readonly sql8005site4nownetContext _db;
        private readonly EncryptService encrypt;

        public UserController(sql8005site4nownetContext context, EncryptService encrypt)
        {
            _db = context;
            this.encrypt = encrypt;
        }
        
        public IActionResult StoreLogin()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = _db.RestaurantTable.Where(x => x.RestaurantAccount == model.RestaurantAccount).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Error = "帳號已經存在";
                return View("Register");
            }

            _db.RestaurantTable.Add(new RestaurantTable()
            {
                RestaurantAccount = model.RestaurantAccount,
                RestaurantPassword = model.RestaurantPassword,
                RestaurantName = model.RestaurantName,
                RestaurantAddress = model.RestaurantAddress,
                //RestaurantPhone=model.RestaurantPhone,
                RestaurantEmail = model.RestaurantEmail,
                //OpeningHours=model.OpeningHours
                RestaurantDescription = model.RestaurantDescription,
                //RestaurantPicture=model.RestaurantPicture

            });
            _db.SaveChanges();//存入資料庫

            //寄信

            AesValidationDto obj = new AesValidationDto(model.RestaurantAccount, DateTime.Now.AddDays(3));
            string jString = JsonSerializer.Serialize(obj);
            var code = encrypt.AesEncryptToBase64(jString);

            var mail = new MailMessage()
            {
                From = new MailAddress("thm10105@gmail.com"),
                Subject = "啟用網站驗證",
                Body = $@"請點這<a src='https://localhost:7163/user/enable?code={code}'>這裡</a>來啟用你的帳號",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
            };
            mail.To.Add(new MailAddress(model.RestaurantAccount));
            using (var sm = new SmtpClient("smtp.gmail.com", 587))
            {
                sm.EnableSsl = true;
                sm.Credentials = new NetworkCredential("thm10105@gmail.com", "csharpthm101");
                sm.Send(mail);
            }
            return View(model);
        }

        public async Task<IActionResult> Enable(string code)
        {
            var str = encrypt.AesDecryptToString(code);
            return Ok($@"code:{code} str:{str}");
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = _db.RestaurantTable.Where(x => x.RestaurantAccount == model.Account &&
            x.RestaurantPassword == model.Password).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "帳號密碼錯誤";
                return View();
            }
            //通行證
            //可以放List?
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.RestaurantName),
                new Claim(ClaimTypes.Role, "User"),
            };
            //做憑證
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
            return RedirectToAction("Index", "home");//到另一個控制器
                                                     //return RedirectToAction("Index", "Users", new { area = "Administration" });

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "home");
        }

        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegisterViewModel model)
        {
            RestaurantTable user = _db.RestaurantTable.Where(x => x.RestaurantAccount == model.RestaurantAccount).FirstOrDefault();
            user.RestaurantPassword = model.RestaurantPassword;
            //user.RestaurantAddress= model.RestaurantAddress;
            user.RestaurantPhone = model.RestaurantPhone;
            //user.RestaurantEmail = model.RestaurantEmail;
            //user.OpeningHours=model.OpeningHours;
            user.RestaurantDescription = model.RestaurantDescription;
            user.RestaurantPicture = model.RestaurantPicture;
            //if (user != null)
            //{
            //	ViewBag.Error = "帳號已經存在";
            //	return View("Register");
            //}

            //_db.RestaurantTable.Add(new RestaurantTable()
            //{
            //	RestaurantAccount = model.RestaurantAccount,
            //	RestaurantPassword = model.RestaurantPassword,
            //	RestaurantName = model.RestaurantName,
            //	RestaurantAddress = model.RestaurantAddress,
            //	//RestaurantPhone=model.RestaurantPhone,
            //	RestaurantEmail = model.RestaurantEmail,
            //	//OpeningHours=model.OpeningHours
            //	RestaurantDescription = model.RestaurantDescription,
            //	//RestaurantPicture=model.RestaurantPicture

            //});
            _db.SaveChanges();//存入資料庫

            return View(model);
        }
    }
}
