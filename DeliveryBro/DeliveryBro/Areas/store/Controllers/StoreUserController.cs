using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Services;
using DeliveryBro.Models;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DeliveryBro.Areas.store.ViewModels;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Facebook;
using NuGet.Protocol.Plugins;

namespace DeliveryBro.Areas.store.Controllers
{
	[Area("store")]
	public class StoreUserController : Controller
	{
		private readonly sql8005site4nownetContext _db;
		private readonly EncryptService _encrypt;
		private readonly PasswordEncyptService _passwordEncyptService;

		public StoreUserController(sql8005site4nownetContext context, EncryptService encrypt, PasswordEncyptService passwordEncyptService)
		{
			_db = context;
			_encrypt = encrypt;//email加密
			_passwordEncyptService = passwordEncyptService;//密碼加密
		}
		public IActionResult Login()
		{
			return View();
		}
		public IActionResult Register()
		{
			return View();
		}

		public IActionResult Error()
		{
			return View();
		}

		public IActionResult SendEmail()
		{
			return View();
		}

		//寄驗證信
		[HttpPost]
		public IActionResult SendEmail(string RestaurantEmail)
		{
			RestaurantTable temp = _db.RestaurantTable.FirstOrDefault(x => x.RestaurantEmail == RestaurantEmail);
			if (temp == null)
			{
				ViewBag.Error = "此Email尚未註冊";
				return View("Register");
			}
			AesValidationDto obj = new AesValidationDto(RestaurantEmail, DateTime.Now.AddDays(3));//現在時間往後加三天
			string jString = JsonSerializer.Serialize(obj);//Obj轉成Json格式(出來是string)
			var code = _encrypt.AesEncryptToBase64(jString);//加密

			var mail = new MailMessage()
			{
				From = new MailAddress("linyh22031@gmail.com"),
				Subject = "重設密碼驗證",
				Body = $@"請點擊 <a href='https://localhost:7163/StoreUser/ResetPwd?code={code}'>這裡</a> 重設您的密碼",
				IsBodyHtml = true,
				BodyEncoding = Encoding.UTF8,//整個網頁的編碼
			};
			mail.To.Add(new MailAddress(RestaurantEmail));

			try
			{
				using (var sm = new SmtpClient("smtp.gmail.com", 587))
				{
					sm.EnableSsl = true;
					sm.Credentials = new NetworkCredential("linyh22031@gmail.com", "xluyufcxhuvsdknp");
					sm.UseDefaultCredentials = false;
					//sm.Credentials = Netcre;
					sm.Send(mail);
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return View();
		}

		public async Task<IActionResult> Enable(string code)
		{
			var str = _encrypt.AesDecryptToString(code);
			var obj = JsonSerializer.Deserialize<AesValidationDto>(str);
			if (DateTime.Now > obj.ExpiredDate)
			{
				return BadRequest("過期");
			}
			//var user = _db.RestaurantTable.FirstOrDefault(x => x.RestaurantAccount == obj.Account);
			//if (user != null)
			//{
			//	user.IsActive = true;
			//	_db.SaveChanges();
			//}
			return Ok($@"code:{code} str:{str}");
		}

		//重設密碼的畫面
		public IActionResult ResetPwd()
		{
			return View();
		}
		[HttpPost]
		//在密碼重設頁面，您需要輸入新的密碼兩次以確認一致性。
		public async Task<IActionResult> ResetPwd(string ResaurantEmail)
		{
			//之後Email要加密 找Email有沒有存在資料庫
			var rpwd = _db.RestaurantTable.FirstOrDefault(x => x.RestaurantEmail == ResaurantEmail);
			if (rpwd == null)
			{
				ViewBag.Error = "此Email尚未註冊";
				return View("ResetPwd");

			}
			
			//填密碼的頁
			
			return RedirectToAction("ResetPwd");

		}
		//public IActionResult Newpwd(string pwd)
		//{
		//	var query=_db.RestaurantTable.
		//}

		//FB第三方登入
		public IActionResult FacebookLogin()
		{
			var prop = new AuthenticationProperties
			{
				RedirectUri = Url.Action("FaceBookResponse")
			};
			return Challenge(prop, FacebookDefaults.AuthenticationScheme);
		}

		public async Task<IActionResult> FaceBookResponse()
		{
			var result = await HttpContext.AuthenticateAsync("Facebook");
			if (result.Succeeded)
			{
				var claims = result.Principal.Claims.Select(x => new  //new一個匿名物件
				{
					x.Type,
					x.Value
				});
				//return Json(claims);
				return View("Register", "StoreUser");//之後改成可以跳轉到商家首頁
			}
			return Ok();
			//RedirectToAction("Index", "Home");
		}

		//忘記密碼
		public IActionResult ForgetPwd()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgetPwd(string ResaurantEmail)//這裡Restaurant要跟前端的name一樣
		{
			var fpwd = _db.RestaurantTable.FirstOrDefault(x => x.RestaurantEmail == ResaurantEmail);
			if (fpwd == null)
			{
				ViewBag.Error = "此Email尚未註冊";
				return View("ForgetPwd");

			}
			//回傳重設密碼的頁面
			return RedirectToAction("ResetPwd");
			//比對信箱
			//return View("Login");
		}


		//註冊
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			//======================================
			//判斷輸入密碼及確認密碼欄位是否一致


			if (model.RestaurantPassword != model.ConfirmRestaurantPassword)
			{
				return Ok("密碼輸入不一致");
			}


			//======================================
			var user = _db.RestaurantTable.Where(x => x.RestaurantAccount == model.RestaurantAccount).FirstOrDefault();

			if (user != null)
			{
				ViewBag.Error = "帳號已經存在";
				//return View("Register");
				return View("Login");
			}
			
			_db.RestaurantTable.Add(new RestaurantTable()
			{
				RestaurantAccount = model.RestaurantAccount,
				RestaurantPassword = _passwordEncyptService.PasswordEncrypt(model.RestaurantPassword),//用類別裡面的PasswordEncrypt方法加密
				RestaurantName = model.RestaurantName,
				RestaurantAddress = model.RestaurantAddress,
				RestaurantPhone = model.RestaurantPhone,
				RestaurantEmail = model.RestaurantEmail,
				//OpeningHours = model.OpeningHours,
				//RestaurantDescription = model.RestaurantDescription,
				//RestaurantPicture = model.RestaurantPicture//改一個函式(要寫在外面 再回來跑這一行)

			});
			//存入資料庫
			try
			{
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				return View("Error.cshtml");//如果無法存入資料庫 報錯的畫面
			}

			return View("store/Views/Home/Index.cshtml");
		}



		//登入
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			////驗證ViewModel的欄位屬性
			if (ModelState.IsValid)
			{

				//FirstOrDefault找尋資料表中的第一筆資料 有資料回傳第一筆或是沒資料回傳null
				//找RestaurantTable資料表篩選符合RestaurantAccount及RestaurantPassword的條件
				RestaurantTable user = _db.RestaurantTable.FirstOrDefault(x => x.RestaurantAccount == model.Account &&
							x.RestaurantPassword ==model.Password);
				if (user == null)
				{
					ViewBag.Error = "帳號或密碼錯誤";
					return View("Login");
				}

				//通行證
				var claims = new List<Claim>(){
				new Claim(ClaimTypes.Name, user.RestaurantName),
				new Claim(ClaimTypes.Role, "Store"),
				new Claim("RestaurantId",user.RestaurantId.ToString())
				};
				//做憑證
				var claimsIdentity = new ClaimsIdentity(claims, "StoreAuthenticationScheme");
				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
				await HttpContext.SignInAsync("StoreAuthenticationScheme", claimsPrincipal);
				return RedirectToAction("StoreInfo", "Home");//RedirectToAction("Error", "StoreUser");//到另一個控制器
														 //return RedirectToAction("Index", "Users", new { area = "Administration" });
			}
			ViewBag.ErrorMessage = "輸入的內容有誤";
			return View("Login");
		}

		//登出
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("StoreAuthenticationScheme");
			return RedirectToAction("Login", "StoreUser");
		}

		public IActionResult Edit()
		{
			return View();
		}

		//[HttpPost]
		//public async Task<IActionResult> Edit(RegisterViewModel model)
		//{
		//	RestaurantTable user = _db.RestaurantTable.Where(x => x.RestaurantAccount == model.RestaurantAccount).FirstOrDefault();
		//	user.RestaurantPassword = model.RestaurantPassword;
		//	//user.RestaurantAddress= model.RestaurantAddress;
		//	user.RestaurantPhone = model.RestaurantPhone;
		//	//user.RestaurantEmail = model.RestaurantEmail;
		//	//user.OpeningHours=model.OpeningHours;
		//	user.RestaurantDescription = model.RestaurantDescription;
		//	user.RestaurantPicture = model.RestaurantPicture;
		//	//if (user != null)
		//	//{
		//	//	ViewBag.Error = "帳號已經存在";
		//	//	return View("Register");
		//	//}

		//	//_db.RestaurantTable.Add(new RestaurantTable()
		//	//{
		//	//	RestaurantAccount = model.RestaurantAccount,
		//	//	RestaurantPassword = model.RestaurantPassword,
		//	//	RestaurantName = model.RestaurantName,
		//	//	RestaurantAddress = model.RestaurantAddress,
		//	//	//RestaurantPhone=model.RestaurantPhone,
		//	//	RestaurantEmail = model.RestaurantEmail,
		//	//	//OpeningHours=model.OpeningHours
		//	//	RestaurantDescription = model.RestaurantDescription,
		//	//	//RestaurantPicture=model.RestaurantPicture

		//	//});
		//	_db.SaveChanges();//存入資料庫

		//	return View(model);
		//}
	
	}
}
