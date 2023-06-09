using DeliveryBro.Areas.admin.Models.ViewModel;
using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DeliveryBro.Areas.admin.Controllers.ApiControllers
{

	[Route("api/OrderManagement/[action]")]
	[ApiController]
	public class OrderManagementController : ControllerBase
	{
		private readonly sql8005site4nownetContext _db;
		public OrderManagementController(sql8005site4nownetContext context)
		{
			_db = context;
		}
        //var nextPage = context.Posts
		//	.OrderBy(b => b.PostId)
		//	.Skip(position)
		//	.Take(10)
		//	.ToList();
		[HttpGet("{page}")]
		public async Task<OrderManagementDTO> GetOrder(int page)
		{

			var quantity = 10; // 一頁幾筆
			var count = 0; // 全部幾筆
  			var query = _db.CustomerOrderTable				
			 .OrderBy(x => x.OrderId); //遞增排序
			count = await query.CountAsync(); // 先查全部有幾筆

			var Order = await query
			   .Skip(quantity * (page - 1)) //跳過一頁幾筆*頁碼
			   .Take(quantity) // 取幾筆
			   .Select(Order => new OrderDTO
			   {
				   OrderId = Order.OrderId,
				   OrderDate = Order.OrderDate,
				   OrderStatus = Order.OrderStatus,
				   AmountAfterDiscount = Order.AmountAfterDiscount,
				   CustomerAddress = Order.CustomerAddress
			   }).ToListAsync();

			return new OrderManagementDTO
			{
				Count = count,
				Items = Order
			};
		}
		[HttpPut]
		public async Task<string> PutOrder(int id, [FromBody] OrderDTO oddetailsdto)
		{
			if (id != oddetailsdto.OrderId)
			{
				return "修改失敗!";
			}
			var order = await _db.CustomerOrderTable.FindAsync(id);

			if (order == null)
			{
				return "修改失敗!"; // 訂單不存在，返回失敗訊息
			}

			order.OrderId = oddetailsdto.OrderId;
			order.OrderDate = oddetailsdto.OrderDate;
			order.CustomerAddress = oddetailsdto.CustomerAddress;
			order.AmountAfterDiscount = oddetailsdto.AmountAfterDiscount;

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_db.CustomerOrderTable.Any(m => m.OrderId == oddetailsdto.OrderId))
				{
					return "修改失敗!"; // 訂單不存在，返回失敗訊息
				}
				else
				{
					throw; // 其他並行更新異常，重新拋出
				}
			}

			return "修改成功!"; // 更新成功，返回成功訊息
		}


		[HttpDelete("{Id:int}")]
		public async Task<string> Delete(int Id)
		{
			var r = await _db.CustomerOrderTable.Include(o => o.OrderDetailsTable).Where(x => x.OrderId == Id).FirstOrDefaultAsync();
			if (r == null)
			{
				return "找不到資料";
			}

			foreach (var item in r.OrderDetailsTable)
			{
				_db.OrderDetailsTable.Remove(item);
			}

			_db.CustomerOrderTable.Remove(r);
			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				return "刪除失敗!" + ex.Message;
			}

			return "刪除成功!";
		}




		private bool OrderExists(int id)
		{
			return (_db.CustomerOrderTable?.Any(e => e.OrderId == id)).GetValueOrDefault();
		}

	}
}
