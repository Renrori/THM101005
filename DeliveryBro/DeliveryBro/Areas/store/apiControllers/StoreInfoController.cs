using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Extensions;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace DeliveryBro.Areas.store.apiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Store", AuthenticationSchemes = "StoreAuthenticationScheme")]
    public class StoreInfoController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public StoreInfoController(sql8005site4nownetContext context)
        {
            _context = context;
        }

        // GET: api/RestaurantTables
        [HttpGet]
        public async Task<IQueryable<StoreInfoDTO>> GetRestaurantTable()
        
        {
			var id = User.GetId();
			return _context.RestaurantTable.Where(x => x.RestaurantId == id).Select(x => new StoreInfoDTO
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
        [HttpPut]
        public async Task<string> PutRestaurantTable(IFormCollection form)
        {
			var id = User.GetId();
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

        [HttpPut("status")]
        public async Task<string> StoreStatusChange(StoreStatusDTO statusDTO)
        {
			var id = User.GetId();
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
			var id = User.GetId();
			var orders = _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
                .Where(x => x.RestaurantId == id && x.OrderStatus == "completed").ToList();
			var query = orders.GroupBy(x => x.OrderDate.Month, (month, order) => new
			{
				Month = month.ToString(),
				Order = order.SelectMany(i => i.OrderDetailsTable).GroupBy(n => n.DishName, (name, number) => new
				{
					Name = name,
					Number = number.Count()
				}).ToList()

			}).ToList();
			var data = query.SelectMany(x => x.Order.Select(o => new
			{
				Month = x.Month,
				Name = o.Name,
				Number = o.Number
			})).ToList();
			var months = data.Select(m => m.Month).Distinct().OrderBy(m => m).ToList();
			var disharr = data.Select(d => d.Name).Distinct().OrderBy(m => m).ToList();
			var result = new List<object[]>();
			var headerRow = new object[months.Count() + 1];
			headerRow[0] = "product";
			for (int i = 0; i < months.Count(); i++) headerRow[i + 1] = months[i];
			result.Add(headerRow);

			foreach (var d in disharr)
			{
				var dishData = new object[months.Count() + 1];
				dishData[0] = d;
				for (int i = 0; i < months.Count(); i++)
				{
					var month = months[i];
					var num = data.Where(n => n.Month == month && n.Name == d).Sum(s => s.Number);
					dishData[i + 1] = num;
				}
				result.Add(dishData);
			}

			return Ok(result);
		}

		[HttpGet("topselling")]
        public Object GetTopSelling()
        {
			var id = User.GetId();
			List<DishDTO> list = new List<DishDTO>();
            Dictionary<int,int> IdLocation=new Dictionary<int,int>();
            var menuName = _context.MenuTable.Where(x => x.RestaurantId == id).Select(x => x.DishName).ToList();
			var menuId = _context.MenuTable.Where(x => x.RestaurantId == id).Select(x => x.DishId).ToList();
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
            var itemcount = _context.CustomerOrderTable
                .Where(x => x.RestaurantId == id &&x.OrderStatus=="completed")
                .Select(x => new
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
			var id = User.GetId();
            var query = _context.CustomerOrderTable
                .Where(x => x.RestaurantId == id && x.OrderDate.Date.ToString() == DateTime.UtcNow.Date.ToString()&&x.OrderStatus=="completed")
                .GroupBy(x=>x.OrderDate.Date.ToString())
                .Select(q => new
                {
                    Date = q.Key,
                    Orders = _context.CustomerOrderTable.Where(x => x.RestaurantId == id && x.OrderStatus == "completed")
                            .Count(x=>x.OrderDate.Date.ToString() == DateTime.Today.Date.ToString()),
                    Revenue =q.SelectMany(o=>o.OrderDetailsTable).Sum(o=>o.Subtotal)
                }) ;
            
            return Ok(query);
        }

        private bool RestaurantTableExists(Guid id)
        {
            return (_context.RestaurantTable?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }
    }
}
