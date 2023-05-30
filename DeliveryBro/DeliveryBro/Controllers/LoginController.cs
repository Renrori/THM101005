﻿using DeliveryBro.Models;
using DeliveryBro.ViewModels.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using DeliveryBro.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using NuGet.Versioning;

namespace DeliveryBro.Controllers
{

    public class LoginController : Controller
    {
        private readonly sql8005site4nownetContext _context;
        private readonly PasswordEncyptService _passwordEncyptService;

        public LoginController(sql8005site4nownetContext context,PasswordEncyptService passwordEncyptService)
        {
            _context = context;
            _passwordEncyptService= passwordEncyptService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("UserAccount, UserPassword")] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                //先將內容加密
                var encyptpassword = _passwordEncyptService.PasswordEncrypt(login.UserPassword);
                //查找是否存在此ID用戶
                var checkId = _context.CustomersTable.Any(x => x.CustomerAccount == login.UserAccount);
                //對應ID的資料
                var user = _context.CustomersTable.FirstOrDefault(x=>x.CustomerAccount == login.UserAccount && x.CustomerPassword == encyptpassword);
                 if (!checkId || user ==null)
                {
                    ViewBag.ErrorMessage = "帳號或密碼錯誤";
                    return View(login);
                }
                
                var claims = new List<Claim>() {
                     new Claim(ClaimTypes.Name, user.CustomerName),
                     new Claim (ClaimTypes.Role,"User"),
                     new Claim("CustomerId",user.CustomerId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //ClaimsPrincipal也可以List
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorMessage = "輸入的內容有誤";
            return View(login);
        }

        public IActionResult FacebookLogin()
        {
            //prop

            var prop = new AuthenticationProperties
            {
                RedirectUri =Url.Action("FacebookRes")

            };
            //發請求內容並傳入
            return Challenge(prop,FacebookDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> FacebookRes()
        {
            //非同步去呼叫
            var result =await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
            //如果驗證成功
            if(result.Succeeded)
            {
                var claimsId = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var claimName = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                var claimsEmail = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

                var claims = new List<Claim>() {
                     new Claim(ClaimTypes.Name, claimName),
                     new Claim (ClaimTypes.Role,"User"),


                     //new Claim("CustomerId",user.CustomerId.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //ClaimsPrincipal也可以List
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }
            return Ok();
        }

        public IActionResult GoogleLogin()
        {
            //prop

            var prop = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleRes")

            };
            //發請求內容並傳入
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleRes()
        {   //非同步等待
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            //如果驗證成功
            if (result.Succeeded)
            {
                var claimsId = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var claimName = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                var claimsEmail = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

                var claims = new List<Claim>() {
                     new Claim(ClaimTypes.Name, claimName),
                     new Claim (ClaimTypes.Role,"User"),


                     //new Claim("CustomerId",user.CustomerId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //ClaimsPrincipal也可以List
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                //var claims = result.Principal.Claims.Select(x => new
                //{
                //    //打印Claims物件
                //    x.Type,
                //    x.Value,
                //});

    

                return RedirectToAction("Index", "Home");
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //public bool OAuthCheck()
        //{
        //    var claimsId = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        //    var claimName = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
        //    var claimsEmail = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        //    var checkOAuth = _context.CustomersTable.Any(c => c.CustomerAccount == claimsId && c.CustomerEmail == claimsEmail);
        //    if (!checkOAuth)
        //    {
        //        CustomersTable customer = new CustomersTable
        //        {
        //            CustomerAccount = claimsId,
        //            CustomerPassword = "",
        //            CustomerName = claimName,
        //            CustomerEmail = claimsEmail,
        //        };
        //    }

        //}
        public IActionResult Register()
        {
            return View();
        }

	}
}
