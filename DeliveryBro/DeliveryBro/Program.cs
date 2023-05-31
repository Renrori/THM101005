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
			//��������
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(opt =>
				{
					opt.Cookie.HttpOnly = true;
					opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
					opt.LoginPath = "/user/Login";//����cookie��n���઺�e��(�n�J�O���ӵe��)
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
				//�K�X�]�w
				options.Password.RequireDigit = true;//�K�X�ݤ��ݭn�]�t�Ʀr
				options.Password.RequireLowercase = true; //�K�X�O�_�����]�t�p�g
				options.Password.RequireUppercase = true; //�K�X�O�_�����]�t�j�g
				options.Password.RequiredLength = 8; //�K�X���ץ����ŦX8�H�W

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //���o�γ]�w TimeSpan �o����w�ɡA�ϥΪ̾D����w�C �w�]�� 5 �����C	
				options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //���ҨϥΪ̦W�٪��ϥΪ̦W�٤����\���r���M��
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