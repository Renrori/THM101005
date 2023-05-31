﻿using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Areas.store.SubscribeTableDependency;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace DeliveryBro.Areas.store.apiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreInfoController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;
        private readonly subscribeOrder _subscribeOrder;

        public StoreInfoController(sql8005site4nownetContext context,subscribeOrder subscribeOrder)
        {
            _context = context;
            _subscribeOrder = subscribeOrder;
        }

        // GET: api/RestaurantTables
        [HttpGet]
        public async Task<IQueryable<StoreInfoDTO>> GetRestaurantTable()
        {
            return _context.RestaurantTable.Where(x => x.RestaurantId == 3).Select(x => new StoreInfoDTO
            {
                RestaurantId = x.RestaurantId,
                RestaurantDescription = x.RestaurantDescription,
                RestaurantEmail = x.RestaurantEmail,
                RestaurantName = x.RestaurantName,
                RestaurantAddress = x.RestaurantAddress,
                RestaurantPhone = x.RestaurantPhone,
                RestaurantPicture = x.RestaurantPicture,
                OpeningHours = x.OpeningHours,
                EndHours= x.EndHours,
                PrepareTime=x.PrepareTime,
                RestaurantStatus = x.RestaurantStatus
            });
        }

        private async Task<byte[]> Getpic(IFormFile file)
        {
            if (file != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }
        private async Task SetPic(RestaurantTable store, IFormFile file)
        {
            store.RestaurantPicture = await Getpic(file);
        }
        // PUT: api/RestaurantTables/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutRestaurantTable(int id, IFormCollection form)
        {

            RestaurantTable store = await _context.RestaurantTable.FindAsync(id);
            store.RestaurantDescription = form["RestaurantDescription"];
            DateTime dateTimeOpen = DateTime.ParseExact(form["OpeningHours"], "HH:mm", CultureInfo.InvariantCulture);
            store.OpeningHours = dateTimeOpen.TimeOfDay;
			DateTime dateTimeEnd = DateTime.ParseExact(form["EndHours"], "HH:mm", CultureInfo.InvariantCulture);
			store.EndHours = dateTimeEnd.TimeOfDay;
            store.PrepareTime = int.Parse(form["PrepareTime"]);
			IFormFile picfile = form.Files.GetFile("RestaurantPicture");
            if (picfile != null)
                await SetPic(store, picfile);
            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTableExists(id))
                {
                    return "無法更改";
                }
                else
                {
                    throw;
                }
            }

            return "更改成功";
        }

        [HttpPut("status/{id}")]
        public async Task<string> StoreStatusChange(int id, StoreStatusDTO statusDTO)
        {
            if (id != statusDTO.RestaurantId)
            {
                return "無法更改營業狀態";
            }
            RestaurantTable store = await _context.RestaurantTable.FindAsync(id);
            store.RestaurantStatus = statusDTO.RestaurantStatus;
            _context.Entry(store).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTableExists(id))
                    return "無法更改營業狀態";
                else
                    throw;
            }
            return "營業狀態修改成功";
        }

        [HttpGet("dishcount")]
        public Object GetDishCount()
        {
            var query = _context.CustomerOrderTable.Where(x => x.RestaurantId == 3).Include(x => x.OrderDetailsTable)
                .GroupBy(x => x.OrderDate.Month).Select(q => new DishMonthlyChartDTO
                {
                    Month = q.Key,
                    Dish = q.SelectMany(od => od.OrderDetailsTable).GroupBy(od => od.DishName).Select(n => new DishEChartsDTO
                    {
                        DishName = n.Key,
                        Number = n.Count()
                    }).ToList()
                });

            return Ok(query);
        }

        [HttpGet("topselling")]
        public Object GetTopSelling()
        {
            List<DishDTO> list = new List<DishDTO>();
            Dictionary<int,int> IdLocation=new Dictionary<int,int>();
            var menuName = _context.MenuTable.Where(x => x.RestaurantId == 3).Select(x => x.DishName).ToList();
			var menuId = _context.MenuTable.Where(x => x.RestaurantId == 3).Select(x => x.DishId).ToList();
			for (var i=0; i < menuId.Count; i++)
            {
                DishDTO dish = new DishDTO();
                dish.name= menuName[i];
                dish.Id= menuId[i];
                dish.quantity = 0;
                dish.subtotal = 0;
                list.Add(dish);
            }
            for(var i = 0; i < menuId.Count(); i++)
            {
				IdLocation.Add(menuId[i], i);
            }
            var itemcount = _context.CustomerOrderTable.Where(x => x.RestaurantId == 3).Select(x => new
            {
                itemdetail = x.OrderDetailsTable.Select(i => new
                {
                    Id = i.DishId,
                    name = i.DishName,
                    quantity = i.Quantity,
                    subtotal = i.Subtotal
                }).ToList()
            }).ToList() ;
            
            foreach( var ic in itemcount)
            {
                foreach( var i in ic.itemdetail)
                {
                    if(IdLocation.ContainsKey(i.Id)) 
                    {
                        int value = IdLocation[i.Id];
                        list[value].subtotal += i.subtotal;
						list[value].quantity += i.quantity;
					}
                }
            }

            return Ok(list.OrderByDescending(x=>x.quantity));
        }
        [HttpGet("orderscount")]
        public Object GetMointhliyOrders()
        {
            _subscribeOrder.Subscribe();
			var query = _context.CustomerOrderTable
                .Where(x => x.RestaurantId == 3 && x.OrderDate.Date.ToString() == DateTime.Today.Date.ToString())
                .GroupBy(x=>x.OrderDate.Date.ToString())
                .Select(q => new
                {
                    Date = q.Key,
                    Orders = _context.CustomerOrderTable.Where(x => x.RestaurantId == 3  )
                            .Count(x=>x.OrderDate.Date.ToString() == DateTime.Today.Date.ToString()),
                    Revenu =q.SelectMany(o=>o.OrderDetailsTable).Sum(o=>o.Subtotal)
                }) ;
            
            return Ok(query);
        }

        private bool RestaurantTableExists(int id)
        {
            return (_context.RestaurantTable?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }
    }
}
