﻿using DeliveryBro.Hubs;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TableDependency.SqlClient;

namespace DeliveryBro.Areas.store.Controllers
{
	[Area("store")]
	[Authorize(Roles = "Store", AuthenticationSchemes = "StoreAuthenticationScheme")]
	public class HomeController : Controller
	{
		
        private readonly ILogger<HomeController> _logger;
		

		public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
		{
			return View();
		}
		public IActionResult StoreInfo()
		{
			return View();
		}
		public IActionResult OrdersInTime()
		{
			
			return View();
		}
		public IActionResult HisOrder()
		{
			return View();
		}
		
		public IActionResult Menu()
		{
			//var claims = HttpContext.User.Claims;
			return View();
		}
		public IActionResult HelpCenter()
		{
			return View();
		}
	}
}
