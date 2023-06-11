using DeliveryBro.Areas.store.ViewModels;
using DeliveryBro.Extensions;
using DeliveryBro.Models;
using DeliveryBro.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace DeliveryBro.Areas.store.apiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StoreLoginController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
		private readonly EncryptService _encrypt;
		private readonly PasswordEncyptService _passwordEncyptService;
		public StoreLoginController(sql8005site4nownetContext context, EncryptService encrypt, PasswordEncyptService passwordEncyptService)
		{
			_context = context;
			_encrypt = encrypt;//email加密
			_passwordEncyptService = passwordEncyptService;//密碼加密
		}

		

		[HttpPost("isConfirm/StoreLogin")]
		//[ValidateAntiForgeryToken]
		public async Task<string> Login(LoginViewModel model)
		{
			
			//FirstOrDefault找尋資料表中的第一筆資料 有資料回傳第一筆或是沒資料回傳null
			//找RestaurantTable資料表篩選符合RestaurantAccount及RestaurantPassword的條件
			RestaurantTable user = _context.RestaurantTable.FirstOrDefault(x => x.RestaurantAccount == model.account &&
						x.RestaurantPassword == _passwordEncyptService.PasswordEncrypt(model.password));
			if (user == null)
			{
				return "帳號或密碼錯誤";
			}

			//通行證
			var claims = new List<Claim>(){
				new Claim(ClaimTypes.Name, user.RestaurantName),
				new Claim(ClaimTypes.Role, "Store"),
				new Claim("Id",user.RestaurantId.ToString())

				};
            //做憑證
            await HttpContext.SignOutAsync("CustomerAuthenticationScheme");
            await HttpContext.SignOutAsync("AdministratorAuthenticationScheme");
            var claimsIdentity = new ClaimsIdentity(claims, "StoreAuthenticationScheme");
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			await HttpContext.SignInAsync("StoreAuthenticationScheme", claimsPrincipal);

			return "登入成功";
		
		}

	}
}
