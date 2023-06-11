using DeliveryBro.Areas.admin.DTO;
using DeliveryBro.Areas.admin.Models;
using DeliveryBro.Areas.admin.Models.ViewModel;
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

		[HttpPost]
		public IEnumerable<OrderBasicDTO> GetOrders(OrdersViewModel query)
		{
			//Include所需要的資料表 等待(餐廳接收前) 接收 完成 可用if else
			IEnumerable<OrderBasicDTO> data = _context.CustomerOrderTable
				.Include(x => x.OrderDetailsTable)
				.Include(x => x.Restaurant)
				.Include(x => x.Driver)
				.Include(x => x.Customer)
				.Select(x => new OrderBasicDTO
				{
					OrderID = x.OrderId,
					OrderDate = x.OrderDate.ToString(),//DateTime轉string
					CustomerID = x.CustomerId,
					DriverID = x.DriverId,
					Total = x.OrderDetailsTable.Sum(x => x.Subtotal),
					RestaurantName = x.Restaurant.RestaurantName,
					OrderStatus = x.OrderStatus,
					
					//OrderDetail裡面所有的明細要放進來(所以要做兩隻)
					CustomerName = x.Customer.CustomerName,
					Subtotal = x.OrderDetailsTable.Sum(x => x.Subtotal),
					DriverName = x.Driver.DriverName,
					Address = x.CustomerAddress,
					Payment = x.Payment,
					Note = x.Note

				}).AsNoTracking().AsEnumerable();



			if (!string.IsNullOrWhiteSpace(query.StartDate)) //接單時間大於等於起始時間(如果有值會做{}內的事情)
			{
				data = data.Where(x => DateTime.Parse(x.OrderDate) >= DateTime.Parse(query.StartDate)).AsEnumerable();
			}

			if (!string.IsNullOrWhiteSpace(query.EndDate)) //接單時間小於結束時間
			{
				data = data.Where(x => DateTime.Parse(x.OrderDate) < DateTime.Parse(query.EndDate)).AsEnumerable();
			}


			if (!string.IsNullOrWhiteSpace(query.StoreName)) //模糊查詢將相符的資料全撈出來
			{
				data = data.Where(x => x.RestaurantName.Contains(query.StoreName)).AsEnumerable();
			}


			if (!string.IsNullOrWhiteSpace(query.Status))
			{
				if (query.Status == "已接收")
				{
					data = data.Where(x => x.OrderStatus == "acepted").AsEnumerable();
				}
				if (query.Status == "已完成")
				{
					data = data.Where(x => x.OrderStatus == "completed").AsEnumerable();
				}
				if (query.Status == "等待中")
				{
					data = data.Where(x => x.OrderStatus == "waiting").AsEnumerable();
				}
			}

			return data;
		}

		//completed已完成/acepted已接收/waiting等待中  共三個狀態

		[HttpPost]
		public string UpdateDetails(UpdateDetailsDTO data) 
		{
			var a = _context.CustomerOrderTable.FirstOrDefault(x=>x.OrderId== data.OrderID);
			if (a != null) 
			{
				a.OrderDate = (DateTime)data.OrderDate;
				a.CustomerAddress = data.CustomerAddress;
				a.OrderStatus = data.OrderStatus;
				a.Note = data.Note;
				_context.SaveChanges();
			}

			return "";
		}
	}
}

