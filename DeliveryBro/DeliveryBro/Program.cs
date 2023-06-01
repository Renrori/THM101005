using DeliveryBro.Data;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeliveryBro.Services;
using DeliveryBro.Areas.store.Hubs;
using DeliveryBro.Areas.store.SubscribeTableDependency;

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

			builder.Services.AddSingleton<subscribeOrder>();
			

			builder.Services.AddSignalR();

			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
			   .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(opt =>
               {
				   
				   opt.LoginPath = "/Login/Index"; //�n�J���|
                   opt.AccessDeniedPath = "/Home/Index"; //�����n�J���|
                   opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
               })
               .AddFacebook(facebookOptions =>  //"Facebook"
               {
                   //���Ϊ��_�r��
                   facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                   facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                   //facebookOptions.Events.TicketReceived
                   facebookOptions.Events.OnCreatingTicket = (x) =>
                   {
                       return Task.CompletedTask;
                   };
               })
               .AddGoogle(googleOptions =>
               {
                   googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                   googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
               });
			//store
			builder.Services.AddAuthentication("StoreAuthenticationScheme")
			   .AddCookie("StoreAuthenticationScheme",opt =>
			   {
				   opt.LoginPath = "/store/StoreUser/Login"; //�n�J���|
				   opt.AccessDeniedPath = "/store/StoreUser/Logout"; //�����n�J���|
				   opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
			   });

			builder.Services.AddHttpContextAccessor();


			builder.Services.AddTransient<EncryptService>();
			builder.Services.AddTransient<PasswordEncyptService>();

            
            builder.Services.AddControllersWithViews();

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
			app.MapHub<OrderHub>("/orderHub");

			app.Run();
		}
	}
}