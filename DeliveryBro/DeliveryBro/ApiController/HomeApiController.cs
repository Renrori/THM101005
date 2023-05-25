﻿using DeliveryBro.Models;
using DeliveryBro.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public HomeApiController(sql8005site4nownetContext context)
        {
            _context = context;
        }

        //抓取有商品的店家回傳
        [HttpGet]
        // Get:/api/HomeApi/
        public async Task<IEnumerable<StoreViewModel>> GetStore()
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
                    StoreName = s.RestaurantName,
                    StoreAddress = s.RestaurantAddress,
                    StoreDescription = s.RestaurantDescription
                })
                .ToListAsync();
            return sm;
        }


        [HttpGet("{id}")]
        // Get:/api/HomeApi/1
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.MenuTable.Include(m => m.Restaurant)
               .Where(m => m.Restaurant.RestaurantId == id && m.DishStatus == "ongoing")
               .Select(p => new MenuViewModel
               {
                   DishId = p.DishId,
                   RestaurantId = p.RestaurantId,
                   DishName = p.DishName,
                   DishPrice = p.DishPrice,
                   DishPicture = p.DishPicture,
                   DishDescription = p.DishDescription,
                   DishCategory = p.DishCategory
               })
               .ToArrayAsync();
            return Ok(product);
        }

        [HttpGet("getpic/{storeId}")]
        // Get:/api/HomeApi/getpic/1
        //叫用商店圖片方法，傳入StoreId回傳圖片
        public async Task<IActionResult> GetPictureStore(int storeId)
        {
            RestaurantTable c = await _context.RestaurantTable.FindAsync(storeId);
            byte[] imgUrl = c?.RestaurantPicture;
            return File(imgUrl, "img/jpeg");

        }

        [HttpPost]
        //Post:/api/HomeApi/
        public async Task<bool> GetOrder([FromBody] OrderViewModel order)
        {
            
            try
            {
                CustomerOrderTable cot = new CustomerOrderTable
                {
                    OrderId = order.OrderId,
                    CustomerAddress = order.CustomerAddress,
                    ShippingFee = order.ShippingFee,
                    Payment = order.Payment,
                    OrderDate =DateTime.UtcNow,
                    OrderStatus = order.OrderStatus,
                    Note = order.Note,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                };
                _context.CustomerOrderTable.Add(cot);
                await _context.SaveChangesAsync();

                int orderId = cot.OrderId;
                
                foreach (var od in order.OrderDetailViewModels)
                {
                    od.OrderId = orderId;
                    OrderDetailsTable odt = new OrderDetailsTable
                    {
                        OrderId = od.OrderId,
                        DishId = od.DishId,
                        OrderDate= DateTime.Today,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Subtotal = od.Subtotal,
                        Total = od.Total,
                        DishName = od.DishName,
                    };
                    _context.OrderDetailsTable.Add(odt);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
