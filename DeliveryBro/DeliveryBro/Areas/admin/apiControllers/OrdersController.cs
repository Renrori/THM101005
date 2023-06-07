using DeliveryBro.Areas.admin.DTO;
using DeliveryBro.Areas.admin.Models;
using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Extensions;
using DeliveryBro.MetaData;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DeliveryBro.Areas.admin.apiControllers
{
	[Route("api/orders/[action]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;	
		public OrdersController(sql8005site4nownetContext context)
        {
			_context = context;
		}

		[HttpPost("{status}")]
		public object GetOrders(string? status)
		{
			//Include所需要的資料表 等待(餐廳接收前) 接收 完成 可用if else
			return _context.CustomerOrderTable
				.Include(x => x.OrderDetailsTable)
				.Include(x => x.Restaurant)
				.Include(x => x.DriverId)
				.Include(x => x.Customer)
				.Where(x => x.OrderStatus == status).Select(x => new OrderViewModel
				{
					OrderID = x.OrderId,
					OrderDate = x.OrderDate.ToString(),//DateTime轉string
					CustomerID = x.CustomerId,
					DriverID = x.DriverId,
					Total = x.OrderDetailsTable.Sum(x => x.Subtotal)

					//OrderDetail裡面所有的明細要放進來(所以要做兩隻)


				});
		}
		//[HttpPost("{status}")]
		//public IQueryable<OdetailDTO> OrderDetail(string? status)
		//{
			
		//	_context.CustomerOrderTable
		//		.Include(x => x.OrderDetailsTable)
		//		.Where(x.OrderStatus == status).OrderByDescending(x => x).Select(x => new OdetailDTO
		//		{
		//			OrderId = x.OrderId,
		//			OrderDate = x.OrderDate.ToString(),
		//			CustomerName = x.Customer.CustomerName,
		//			Note = x.Note,
		//			OrderDetails = x.OrderDetailsTable.Select(d => new OdetailDTO
		//			{
		//				DishName = d.DishName,
		//				UnitPrice = d.UnitPrice,
		//				Quantity = d.Quantity,
		//				Discount = d.Discount,
		//				Subtotal = d.Subtotal
		//			}).ToList(),
		//			Total = x.OrderDetailsTable.Sum(x => x.Subtotal)
		//		});
		//}


	}
}

