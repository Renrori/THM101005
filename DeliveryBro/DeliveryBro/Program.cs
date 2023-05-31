using DeliveryBro.Data;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace DeliveryBro
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			var DeliveryBroconnectionString = builder.Configuration.GetConnectionString("DeliveryBro") ?? throw new InvalidOperationException("Connection string 'DeliveryBro' not found.");
			builder.Services.AddDbContext<sql8005site4nownetContext>(options =>
				options.UseSqlServer(DeliveryBroconnectionString));
			//身分驗證
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(opt =>
				{
					opt.Cookie.HttpOnly = true;
					opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
					opt.LoginPath = "/user/Login";//拿到cookie後要導轉的畫面(登入是哪個畫面)
					opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
					opt.SlidingExpiration = true;
				})

			.AddFacebook(facebookOptions =>
			{
				facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
				facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

			});



			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.AddControllersWithViews();
			//------------
			builder.Services.Configure<IdentityOptions>(options =>
			{
				//密碼設定
				options.Password.RequireDigit = true;//密碼需不需要包含數字
				options.Password.RequireLowercase = true; //密碼是否必須包含小寫
				options.Password.RequireUppercase = true; //密碼是否必須包含大寫
				options.Password.RequiredLength = 8; //密碼長度必須符合8以上

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //取得或設定 TimeSpan 發生鎖定時，使用者遭到鎖定。 預設為 5 分鐘。	
				options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //驗證使用者名稱的使用者名稱中允許的字元清單
				options.User.RequireUniqueEmail = false;
			});
			//------------
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllerRoute(
				name: "store",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
			app.MapControllerRoute(name: "admin",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();

			app.Run();
		}
	}
}