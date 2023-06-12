using DeliveryBro.Areas.store.ViewModels;
using DeliveryBro.Extensions;
using DeliveryBro.Models;
using DeliveryBro.Services;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Drawing;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace DeliveryBro.Areas.store.apiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StoreUserController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
		private readonly EncryptService _encrypt;
		private readonly PasswordEncyptService _passwordEncyptService;
		public StoreUserController(sql8005site4nownetContext context, EncryptService encrypt, PasswordEncyptService passwordEncyptService)
		{
			_context = context;
			_encrypt = encrypt;//email加密
			_passwordEncyptService = passwordEncyptService;//密碼加密
		}


		/// <summary>
		/// 驗證帳號
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		[HttpGet("isConfirm/CheckAccount")]
		public async Task<string> CheckAccount(string? account)
		{
			string result = "正確";
			//查詢table是否有一樣的帳號
			RestaurantTable restaurantTable = _context.RestaurantTable.FirstOrDefault(x => x.RestaurantAccount == account);
			//若有一樣的帳號則回傳錯誤訊息
			if (restaurantTable != null)
			{
				result = "帳號已經存在";
				return result;
			}

			Regex regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{6,15}$");
			bool checkAccount = regex.IsMatch(account);
			if (!checkAccount)
			{
				result = "介於6-15字母且包含數字和英文";
				return result;
			}

			return result;
		}

		/// <summary>
		/// 驗證密碼
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		[HttpGet("isConfirm/CheckPassword")]
		public async Task<string> CheckPassword(string? password)
		{
			string result = "正確";

			Regex regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{6,15}$");
			bool checkPassword = regex.IsMatch(password);
			if (!checkPassword)
			{
				result = "介於6-15字母且包含數字和英文";
				return result;
			}

			return result;
		}

		/// <summary>
		/// 驗證再次輸入的密碼
		/// </summary>
		/// <param name="confirmPassword"></param>
		/// <returns></returns>
		[HttpPost("isConfirm/CheckConfirmPassword")]
		public async Task<string> CheckConfirmPassword(RegisterViewModel inputData)
		{
			string result = "正確";

			if (!string.Equals(inputData.RestaurantPassword, inputData.ConfirmRestaurantPassword))
			{
				result = "密碼輸入不一致";
				return result;
			}

			return result;
		}

		/// <summary>
		/// 驗證電話
		/// </summary>
		/// <param name="phone"></param>
		/// <returns></returns>
		[HttpGet("isConfirm/CheckPhone")]
		public async Task<string> CheckPhone(string? phone)
		{
			string result = "正確";
			if (phone.Length < 6 || phone.Length > 15)
			{
				result = "電話格式錯誤";
				return result;
			}

			return result;
		}

		/// <summary>
		/// 驗證Email
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		[HttpGet("isConfirm/CheckMail")]
		public async Task<string> CheckMail(string? mail)
		{
			string result = "正確";
			Regex regex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
			bool checkEmail = regex.IsMatch(mail);
			if (!checkEmail)
			{
				result = "不符合mail格式";//必須包含一個@字符。
				return result;
			}

			return result;
		}

		/// <summary>
		/// 驗證名字
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		[HttpGet("isConfirm/CheckName")]
		public async Task<string> CheckName(string? name)
		{
			string result = "正確";
			if (name.Length < 1 || name.Length > 20)
			{
				result = "名字格式錯誤";
				return result;
			}

			return result;
		}

		/// <summary>
		/// 驗證地址
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		[HttpGet("isConfirm/CheckAddress")]
		public async Task<string> CheckAddress(string? address)
		{
			string result = "正確";
			if (string.IsNullOrEmpty(address))
			{
				result = "請輸入地址";
				return result;
			}

			return result;
		}


		/// <summary>
		/// 註冊
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		[HttpPost("Add/AddRegister")]
		public string AddRegister(RegisterViewModel model)
		{
			string result = "註冊成功";
			_context.RestaurantTable.Add(new RestaurantTable()
			{
				RestaurantId = Guid.NewGuid(),
				RestaurantAccount = model.RestaurantAccount,
				RestaurantPassword = _passwordEncyptService.PasswordEncrypt(model.RestaurantPassword),//用類別裡面的PasswordEncrypt方法加密
				RestaurantName = model.RestaurantName,
				RestaurantAddress = model.RestaurantAddress,
				RestaurantPhone = model.RestaurantPhone,
				RestaurantEmail = model.RestaurantEmail,
				Latitude = Convert.ToDecimal(Map.Location(model.RestaurantAddress)[0]),
				Longitude= Convert.ToDecimal(Map.Location(model.RestaurantAddress)[1]),
			}) ;
			//存入資料庫
			try
			{
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				return "請重新確認欄位";//如果無法存入資料庫 報錯的畫面
			}

			return result;
		}
	}
}
