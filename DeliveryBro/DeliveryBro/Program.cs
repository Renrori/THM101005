using DeliveryBro.Data;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeliveryBro.Services;
using DeliveryBro.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using Microsoft.OpenApi.Models;
using Hangfire;

namespace DeliveryBro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
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

            builder.Services.AddSignalR();
            //builder.Services.AddSwaggerGen();

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
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            })
            .AddCookie("StoreAuthenticationScheme", opt =>
            {
                opt.Cookie.Name = "storecookie";
                opt.LoginPath = "/store/StoreUser/Login"; //登入路徑
                opt.AccessDeniedPath = "/store/StoreUser/Logout"; //取消登入路徑
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            })
            .AddCookie("AdministratorAuthenticationScheme", opt =>
            {
                opt.Cookie.Name = "admincookie";
                opt.LoginPath = "/admin/Account/Login";
                opt.AccessDeniedPath = "/admin/Account/Logout";
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            })
            .AddCookie("DeliverAuthenticationScheme",opt=>
            {
                opt.Cookie.Name = "delivercookie";
                opt.LoginPath = "/deliver/Home/Login";
                opt.AccessDeniedPath = "/deliver/Home/Logut";
                opt.ExpireTimeSpan= TimeSpan.FromMinutes(30);
            })
            .AddGoogle("Google", googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            })
            .AddFacebook("Facebook", facebookOptions =>  //"Facebook"
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
                option.DefaultPolicy = policybuilder.Build();
            });


            #endregion

            #region DI
            builder.Services.AddSignalR();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();
            builder.Services.AddHangfire(configuration => configuration
		                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
		                    .UseSimpleAssemblyNameTypeSerializer()
		                    .UseRecommendedSerializerSettings()
		                    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
            builder.Services.AddHangfireServer();
            builder.Services.AddSingleton<IUserIdProvider, BasedUserIdProvider>();
            builder.Services.AddTransient<EncryptService>();
            builder.Services.AddTransient<PasswordEncyptService>();
			builder.Services.AddSingleton<OrderNotificationTask>();
            #endregion
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://127.0.0.1:5501",
                                "*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.MapHub<OrderHub>("/orderHub");
            app.MapHub<ChatHub>("/chatHub");
			app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "store",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "admin",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

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