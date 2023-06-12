using DeliveryBro.Areas.deliver.DTO;
using DeliveryBro.Models;
using DeliveryBro.Services;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeliveryBro.Areas.deliver.apiController
{
    [Route("api/[controller]")]
	[ApiController]
	public class DeliverUserController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
		private readonly EncryptService _encrypt;
		private readonly PasswordEncyptService _passwordEncyptService;
		public DeliverUserController(sql8005site4nownetContext context, EncryptService encrypt, PasswordEncyptService passwordEncyptService)
		{
			_context = context;
			_encrypt = encrypt;//email加密
			_passwordEncyptService = passwordEncyptService;//密碼加密
		}



		[HttpPost("Login")]
		//[ValidateAntiForgeryToken]
		public async Task<string> Login(LoginDTO model)
		{
			DriverTable user = _context.DriverTable.FirstOrDefault(x => x.DriverAccount == model.Account &&
						x.DriverPassword == _passwordEncyptService.PasswordEncrypt(model.Password));
			if (user == null)
			{
				return "帳號或密碼錯誤";
			}

			//通行證
			var claims = new List<Claim>(){
				new Claim(ClaimTypes.Name, user.DriverName),
				new Claim(ClaimTypes.Role, "Deliver"),
				new Claim("Id",user.DriverId.ToString())

				};
			//做憑證
			await HttpContext.SignOutAsync("CustomerAuthenticationScheme");
			await HttpContext.SignOutAsync("AdministratorAuthenticationScheme");
			await HttpContext.SignOutAsync("StoreAuthenticationScheme");
			var claimsIdentity = new ClaimsIdentity(claims, "DeliverAuthenticationScheme");
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			await HttpContext.SignInAsync("DeliverAuthenticationScheme", claimsPrincipal);

			return "登入成功";

		}

		[HttpPost]
		public async Task<string> Register(RegisterDTO register)
		{
			var user = _context.DriverTable.FirstOrDefault(x => x.DriverAccount == register.Account);
			if (user != null)
			{
				return "帳號已存在";
			}
			else
			{
				string Accountpermission = await IfAccountIsExist(register.Account);
				string Emailpermission = await IfEmailIsExist(register.Email);
				string Phonepermission = await IfPhoneIsExist(register.Phone);
				string Passwordpermission = await IfPasswordIsValid(register.Password);
				if (Accountpermission == "可以使用" && Emailpermission == "可以使用" &&
					Phonepermission == "可以使用" && Passwordpermission == "可以使用")
				{
					DriverTable driver = new DriverTable
					{
						DriverId = Guid.NewGuid(),
						DriverAccount = register.Account,
						DriverName = register.Name,
						DriverEmail = register.Email,
						DriverPhone = register.Phone,
						DriverPassword = _passwordEncyptService.PasswordEncrypt(register.Password)
					};
					_context.DriverTable.Add(driver);
					try
					{
						await _context.SaveChangesAsync();
					}
					catch (Exception ex)
					{
						return "註冊失敗";
					}

					return "註冊成功!";
					//寄信
					//	var obj = new AesValidationViewModel(register.Account, DateTime.Now.AddDays(1));
					//	var jString = JsonSerializer.Serialize(obj);
					//	var code = _encrypt.AesEncryptToBase64(jString);
					//	var mail = new MailMessage()
					//	{
					//		From = new MailAddress("thm10105@gmail.com"),
					//		Subject = "啟用網站驗證",
					//		Body = @$"<a class=""icon-link"" href=""https://localhost:7163/api/RegisterApi/registermail?code={code}"">
					//					<svg class=""bi"" aria-hidden=""true""><use xlink:href=""#box-seam""></use></svg>
					//					請點這裡來啟用帳號!
					//					</a>",
					//		IsBodyHtml = true,
					//		BodyEncoding = Encoding.UTF8,
					//	};
					//	mail.To.Add(new MailAddress(register.Email));
					//	try
					//	{
					//		using (var sm = new SmtpClient("smtp.gmail.com", 587)) //465 ssl
					//		{
					//			sm.EnableSsl = true;
					//			sm.Credentials = new NetworkCredential("thm10105@gmail.com", "loadozkrsjiocvci");
					//			sm.Send(mail);
					//		}
					//	}
					//	catch (Exception ex)
					//	{
					//		throw;
					//	}
					//	return "請至信箱收取驗證信";
					//}

				}
			}
			return "請重新確認欄位";
		}

		public async Task<IActionResult> registermail(string code)
		{
			var str = _encrypt.AesDecryptToString(code);
			var obj = JsonSerializer.Deserialize<AesValidationViewModel>(str);
			var user = _context.CustomersTable.FirstOrDefault(x => x.CustomerAccount == obj.Account);
			if (user != null && DateTime.Now > obj.ExpiredDate)
			{
				_context.CustomersTable.Remove(user);
				return BadRequest("驗證信已過期");
			}

			return Ok();
		}

		[HttpGet("isExist/account")]
		public async Task<string> IfAccountIsExist(string? account)
		{
			var query = _context.DriverTable.FirstOrDefault(x => x.DriverAccount == account);
			if (query != null) return "已重複";
			bool checkIsAlphaNumeric = IsAlphaNumeric(account);
			if (!checkIsAlphaNumeric || account == "") return "長度要介於6-15且數字和英文字母結合";
			return "可以使用";
		}
		[HttpGet("isExist/password")]
		public async Task<string> IfPasswordIsValid(string? password)
		{
			bool checkIsAlphaNumeric = IsAlphaNumeric(password);
			if (!checkIsAlphaNumeric || password == "") return "長度要介於6-15且數字和英文字母結合";
			return "可以使用";
		}

		[HttpGet("isExist/email")]
		public async Task<string> IfEmailIsExist(string? email)
		{
			var query = _context.DriverTable.FirstOrDefault(x => x.DriverEmail == email);
			if (query != null) return "已重複";
			bool checkIsEmailType = IsEmailType(email);
			if (!checkIsEmailType || email == "") return "不符合mail格式";
			return "可以使用";
		}

		[HttpGet("isExist/phone")]
		public async Task<string> IfPhoneIsExist(string? phone)
		{
			var query = _context.DriverTable.FirstOrDefault(x => x.DriverPhone == phone);
			if (query != null) return "已重複";
			bool checkIsPhoneType = IsPhoneType(phone);
			if (!checkIsPhoneType || phone == "") return "電話為09開頭的10個數字";
			return "可以使用";
		}

		private bool IsAlphaNumeric(string? account)
		{
			Regex regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{6,15}$");
			return regex.IsMatch(account);
		}
		private bool IsPhoneType(string? phone)
		{
			Regex regex = new Regex(@"^09\d{8}$");
			return regex.IsMatch(phone);
		}
		private bool IsEmailType(string? email)
		{
			Regex regex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
			return regex.IsMatch(email);
		}
	}
}

