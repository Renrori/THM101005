using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Areas.store.Hubs;
using DeliveryBro.Areas.store.SubscribeTableDependency;
using DeliveryBro.Extensions;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableDependency.SqlClient;

namespace DeliveryBro.Areas.store.apiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Store", AuthenticationSchemes = "StoreAuthenticationScheme")]
	public class OrdersController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
		private readonly subscribeOrder _subscribeOrder;
		

		public OrdersController(sql8005site4nownetContext context, subscribeOrder subscribeOrder)
		{
			_context = context;
			_subscribeOrder = subscribeOrder;
		}

		[HttpGet]
		public IQueryable<HisOrderDTO> OrderDetail()
		{
			var id = User.GetId(User.GetRole());
			return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				.Where(x => x.RestaurantId == id && x.OrderStatus == "completed").OrderByDescending(x => x).Select(x => new HisOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate.ToUniversalTime().ToLocalTime().ToString(),
					CustomerName = x.Customer.CustomerName,
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
		public async Task<IQueryable<HisOrderDTO>> OrderTime(DateTime? startdate, DateTime? enddate, int? id)
		{

			var query = _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				   .Where(x => x.RestaurantId == 3 && x.OrderStatus == "completed");
			if (startdate.HasValue) query = query.Where(x => x.OrderDate >= startdate);
			if (enddate.HasValue) query = query.Where(x => x.OrderDate <= enddate);
			if (id.HasValue) query = query.Where(x => x.OrderId == id);
			return query.OrderByDescending(x => x).Select(x => new HisOrderDTO
			{
				OrderId = x.OrderId,
				OrderDate = x.OrderDate.ToUniversalTime().ToLocalTime().ToString(),
				CustomerName = x.Customer.CustomerName,
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
		[HttpGet("wait")]
		public IQueryable<HisOrderDTO> WaitingOrder()
		{
			_subscribeOrder.Subscribe();
			var id = User.GetId(User.GetRole());
			return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				.Where(x => x.RestaurantId == id && x.OrderStatus == "waiting").Select(x => new HisOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate.ToUniversalTime().ToLocalTime().ToString(),
					CustomerName = x.Customer.CustomerName,
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
		[HttpGet("acepted")]
		public IQueryable<HisOrderDTO> AceptedOrder()
		{
			var id = User.GetId(User.GetRole());
			return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				.Where(x => x.RestaurantId == id && x.OrderStatus == "acepted").Select(x => new HisOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate.ToUniversalTime().ToLocalTime().ToString(),
					CompletedTime=x.OrderDate.ToUniversalTime().ToLocalTime().AddMinutes((double)x.Restaurant.PrepareTime).ToString(),
					CustomerName = x.Customer.CustomerName,
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
		public async Task<string> OrderStatus(int id, OrderStatusDTO statusDTO)
		{
			if (id != statusDTO.OrderId) return "無法更動訂單";
			CustomerOrderTable order = await _context.CustomerOrderTable.FindAsync(id);
			order.OrderStatus = statusDTO.OrderStatus;
			_context.Entry(order).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return "無法更動訂單";
			}
			return "訂單更動成功";
		}
        [HttpDelete("{id}")]
        public async Task<string> DeleteOrder(int id)
        {

            var Corders = await _context.CustomerOrderTable.FindAsync(id);
			var Odetails = await _context.OrderDetailsTable.Where(o => o.OrderId == id).ToArrayAsync();
            if (Corders == null)
            {
                return "品項刪除成功";
            }
			_context.OrderDetailsTable.RemoveRange(Odetails);
            _context.CustomerOrderTable.Remove(Corders);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return "品項刪除失敗";
            }
            return "品項刪除成功";
        }

    }
}
