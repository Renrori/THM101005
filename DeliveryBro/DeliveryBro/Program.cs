using DeliveryBro.Data;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeliveryBro.Services;
using DeliveryBro.Areas.store.SubscribeTableDependency;
using DeliveryBro.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DeliveryBro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region DB
            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            var DeliveryBroconnectionString = builder.Configuration.GetConnectionString("DeliveryBro") ?? throw new InvalidOperationException("Connection string 'DeliveryBro' not found.");
            builder.Services.AddDbContext<sql8005site4nownetContext>(options =>
                options.UseSqlServer(DeliveryBroconnectionString));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<ApplicationDbContext>();
            #endregion

            #region authentication



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie("CustomerAuthenticationScheme", opt =>
            {
                //opt.Events = new CookieAuthenticationEvents
                //{
                //    OnRedirectToLogin = context =>
                //    {
                //        if (context.Request.Path.StartsWithSegments("/store"))
                //        {
                //            context.Response.Redirect(context.RedirectUri = "/store/StoreUser/Login");
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
                opt.Cookie.Name = "customercookie";
                opt.LoginPath = "/Login/Index"; //登入路徑
                opt.AccessDeniedPath = "/Login/Logout"; //取消登入路徑
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddCookie("StoreAuthenticationScheme", opt =>
            {
                opt.Cookie.Name = "storecookie";
                opt.LoginPath = "/store/StoreUser/Login"; //登入路徑
                opt.AccessDeniedPath = "/store/StoreUser/Logout"; //取消登入路徑
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddCookie("AdministratorAuthenticationScheme", opt =>
            {
                opt.Cookie.Name = "admincookie";
                opt.LoginPath = "/admin/Account/Login";
                opt.AccessDeniedPath = "/admin/Account/Logout";
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddGoogle("google", googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            })
            .AddFacebook("facebook", facebookOptions =>  //"Facebook"
            {
                //取用金鑰字串
                facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                //facebookOptions.Events.TicketReceived
                facebookOptions.Events.OnCreatingTicket = (x) =>
                {
                    return Task.CompletedTask;
                };
            });
            builder.Services.AddAuthorization(option =>
            {
                var policybuilder = new AuthorizationPolicyBuilder("CustomerAuthenticationScheme");
                policybuilder = policybuilder.RequireAuthenticatedUser();
                option.DefaultPolicy=policybuilder.Build();
            });


            #endregion

            #region DI
            builder.Services.AddSignalR();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IUserIdProvider, BasedUserIdProvider>();
            builder.Services.AddTransient<EncryptService>();
            builder.Services.AddTransient<PasswordEncyptService>();
            builder.Services.AddSingleton<subscribeOrder>(); 
            #endregion

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
            app.MapHub<OrderHub>("/orderHub");
            app.MapHub<ChatHub>("/chatHub");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();           

            app.Run();
        }
    }
}