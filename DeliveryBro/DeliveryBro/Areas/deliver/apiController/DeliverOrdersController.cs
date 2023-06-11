using DeliveryBro.Areas.deliver.DTO;
using DeliveryBro.Extensions;
using DeliveryBro.Hubs;
using DeliveryBro.Models;
using DeliveryBro.Services;
using DeliveryBro.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DeliveryBro.Areas.deliver.apiController
{
	[Route("api/[controller]")]
    [Authorize(Roles = "Deliver", AuthenticationSchemes = "DeliverAuthenticationScheme")]
    [ApiController]
	public class DeliverOrdersController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
        private readonly OrderNotificationTask _task;
        public DeliverOrdersController(sql8005site4nownetContext context, OrderNotificationTask task)
		{ 
			_context = context; 
            _task = task;
		}

		[HttpGet("hisorder")]
		public IQueryable CompletedOrders()
		{
			var id = User.GetId();
			return _context.CustomerOrderTable.Include(x=>x.Restaurant)
                .Where(x => x.OrderStatus == "completed" && x.DriverId == id)
				.Select(x => new HistoryOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate.ToLocalTime().ToString(),
					CustomerName = x.Customer.CustomerName,
					RestaurantName=x.Restaurant.RestaurantName,
					Note = x.Note,
					OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
					{
						DishName = d.DishName,
						UnitPrice = d.UnitPrice,
						Quantity = d.Quantity,
						Discount = d.Discount,
						Subtotal = d.Subtotal
					}).ToList(),
					Total = x.OrderDetailsTable.Sum(x => x.Subtotal)
				});
		}
        [HttpGet("time")]
        public async Task<IQueryable<HistoryOrderDTO>> OrderTime(DateTime? startdate, DateTime? enddate, int? id)
        {
            var did = User.GetId();
            var query = _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
                   .Where(x => x.DriverId == did && x.OrderStatus == "completed");
            if (startdate.HasValue) query = query.Where(x => x.OrderDate >= startdate);
            if (enddate.HasValue) query = query.Where(x => x.OrderDate <= enddate);
            if (id.HasValue) query = query.Where(x => x.OrderId == id);
            return query.OrderByDescending(x => x).Select(x => new HistoryOrderDTO
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate.ToLocalTime().ToString(),
                CustomerName = x.Customer.CustomerName,
				RestaurantName = x.Restaurant.RestaurantName,
				Note = x.Note,
                OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
                {
                    DishName = d.DishName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal
                }).ToList(),
                Total = x.OrderDetailsTable.Sum(x => x.Subtotal)
            });
        }
        [HttpGet("deliver")]
        public IQueryable<HistoryOrderDTO> DeliverOrder()
        {
            var id = User.GetId();
            var orderHub = new OrderHub();
            BackgroundJob.Schedule(() => _task.Notify(), TimeSpan.FromSeconds(30));
            var query = _context.CustomerOrderTable
                .Include(x => x.OrderDetailsTable)
                .Include(x=>x.Customer)
                .ThenInclude(x=>x.CustomerAddressTable)
                .Where(x =>  x.OrderStatus == "deliver").Select(x => new HistoryOrderDTO
                {
                    OrderId = x.OrderId,
                    OrderDate = x.OrderDate.ToLocalTime().ToString(),
                    CustomerName = x.Customer.CustomerName,
                    RestaurantName=x.Restaurant.RestaurantName,
                    CustomerId = x.CustomerId,
                    CustomerAddress=x.CustomerAddress,
                    RestaurantAddress=x.Restaurant.RestaurantAddress,
                    OrderStatus= x.OrderStatus,
                    CustomerLocation=new LocationViewModel 
                    {
						Latitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a=>a.CustomerAddress==x.CustomerAddress).Latitude),
						Longitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Longitude)
					},
                    RestaurantLocation=new LocationViewModel
                    {
						Latitude = Convert.ToDouble(x.Restaurant.Latitude),
						Longitude = Convert.ToDouble(x.Restaurant.Longitude)
					},
                    RandCdistance=Map.GetDistance(
                        new LocationViewModel
					    {
                            Latitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Latitude),
                            Longitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Longitude)
                        }, 
                        new LocationViewModel
					    {
						    Latitude = Convert.ToDouble(x.Restaurant.Latitude),
						    Longitude = Convert.ToDouble(x.Restaurant.Longitude)
					    }),
                    Note = x.Note,
                    OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
                    {
                        DishName = d.DishName,
                        UnitPrice = d.UnitPrice,
                        Quantity = d.Quantity,
                        Discount = d.Discount,
                        Subtotal = d.Subtotal
                    }).ToList(),
                    Total = x.OrderDetailsTable.Sum(x => x.Subtotal)
                });
            
            return query;
        }
        [HttpGet("prepared")]
        public IQueryable<HistoryOrderDTO> AceptedOrder()
        {
            var id = User.GetId();
            return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
                .Where(x=>x.OrderStatus == "prepared").Select(x => new HistoryOrderDTO
                {
					OrderId = x.OrderId,
					OrderDate = x.OrderDate.ToLocalTime().ToString(),
					CustomerName = x.Customer.CustomerName,
					RestaurantName = x.Restaurant.RestaurantName,
					CustomerId = x.CustomerId,
					CustomerAddress = x.CustomerAddress,
					RestaurantAddress = x.Restaurant.RestaurantAddress,
                    OrderStatus=x.OrderStatus,
                    CustomerLocation = new LocationViewModel
                    {
                        Latitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Latitude),
                        Longitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Longitude)
                    },
                    RestaurantLocation = new LocationViewModel
                    {
                        Latitude = Convert.ToDouble(x.Restaurant.Latitude),
                        Longitude = Convert.ToDouble(x.Restaurant.Longitude)
                    },
                    RandCdistance = Map.GetDistance(
						new LocationViewModel
						{
                            Latitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Latitude),
                            Longitude = Convert.ToDouble(x.Customer.CustomerAddressTable.FirstOrDefault(a => a.CustomerAddress == x.CustomerAddress).Longitude)
                        },
						new LocationViewModel
						{
                            Latitude = Convert.ToDouble(x.Restaurant.Latitude),
                            Longitude = Convert.ToDouble(x.Restaurant.Longitude)
                        }),
					Note = x.Note,
                    OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
                    {
                        DishName = d.DishName,
                        UnitPrice = d.UnitPrice,
                        Quantity = d.Quantity,
                        Discount = d.Discount,
                        Subtotal = d.Subtotal
                    }).ToList(),
                    Total = x.OrderDetailsTable.Sum(x => x.Subtotal)
                });
        }
        [HttpPut("{id}")]
        public async Task<string> OrderStatus(int id, DeliverOrderStatusDTO statusDTO)
        {
            if (id != statusDTO.OrderId) return "無法接受訂單";
            CustomerOrderTable order = await _context.CustomerOrderTable.FindAsync(id);
            order.OrderStatus = statusDTO.Status;
            if(statusDTO.Status=="deliver")
            {
                order.DriverId = User.GetId();
            }
            _context.Entry(order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return "無法接受訂單";
            }
            return "已接受訂單";
        }
    }
}
