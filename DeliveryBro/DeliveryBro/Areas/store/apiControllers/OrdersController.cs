using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.Areas.store.apiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
		public OrdersController(sql8005site4nownetContext context)
		{
			_context = context;
		}
		[HttpGet]
		public IQueryable<HisOrderDTO> OrderDetail()
		{
			return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				.Where(x => x.RestaurantId == 3 && x.OrderStatus == "completed").OrderByDescending(x => x).Select(x => new HisOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate,
					CustomerName = x.Customer.CustomerName,
					Note = x.Note,
					OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
					{
						DishName = d.Dish.DishName,
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
				OrderDate = x.OrderDate,
				CustomerName = x.Customer.CustomerName,
				Note = x.Note,
				OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
				{
					DishName = d.Dish.DishName,
					UnitPrice = d.UnitPrice,
					Quantity = d.Quantity,
					Discount = d.Discount,
					Subtotal = d.Subtotal
				}).ToList(),
				Total = x.OrderDetailsTable.Sum(x => x.Subtotal)
			});
			//return Ok(orders);
		}
		[HttpGet("wait")]
		public IQueryable<HisOrderDTO> WaitingOrder()
		{
			return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				.Where(x => x.RestaurantId == 3 && x.OrderStatus == "waiting").Select(x => new HisOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate,
					CustomerName = x.Customer.CustomerName,
					Note = x.Note,
					OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
					{
						DishName = d.Dish.DishName,
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
			return _context.CustomerOrderTable.Include(x => x.OrderDetailsTable)
				.Where(x => x.RestaurantId == 3 && x.OrderStatus == "acepted").Select(x => new HisOrderDTO
				{
					OrderId = x.OrderId,
					OrderDate = x.OrderDate,
					CustomerName = x.Customer.CustomerName,
					Note = x.Note,
					OrderDetails = x.OrderDetailsTable.Select(d => new OrderDetailsDTO
					{
						DishName = d.Dish.DishName,
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
	}
}
