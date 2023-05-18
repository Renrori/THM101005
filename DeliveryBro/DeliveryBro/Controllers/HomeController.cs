﻿using DeliveryBro.Models;
using DeliveryBro.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DeliveryBro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly sql8005site4nownetContext _context;

        public HomeController(ILogger<HomeController> logger , sql8005site4nownetContext context)
        {
            _logger = logger;
            _context = context;
        }

        //預設回傳View
        public IActionResult Index()
        {
            return View();
        }
        
        //抓取有商品的店家回傳店家資訊模型(Id,Name)
        public async Task<IActionResult> GetStore()
        {
            var groupMenu = await _context.MenuTable
               //.Where(m => !string.IsNullOrEmpty(m.DishName))
               .GroupBy(m => m.DishId)
               .Select(g => g.Key)
               .ToListAsync();

            var sm = await _context.RestaurantTable
                .Where(s => groupMenu.Contains(s.RestaurantId))
                .Select(s => new StoreViewModel
                {
                    StoreId = s.RestaurantId,
                    StoreName = s.RestauranName,
                    StoreAddress = s.RestaurantAddress,
                    StoreDescription = s.RestauranDescription
                })
                .ToListAsync();
            return Ok(sm);
        }

        public async Task<IActionResult> GetProduct(int id)
        {
            var product = _context.MenuTable.Include(m => m.Restaurant)
               .Where(m => m.Restaurant.RestaurantId == id && m.DishStatus == "ongoing" )
               .Select(p => new MenuViewModel 
               {
                   DishId = p.DishId,
                   RestaurantId = p.RestaurantId,
                   DishName = p.DishName,
                   DishPrice = p.DishPrice,
                   DishDescription = p.DishDescription,
                   DishCategory = p.DishCategory
               })
               .ToArrayAsync();
            return Ok(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}