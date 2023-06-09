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
			CustomerOrderTable od = await _db.CustomerOrderTable.FindAsync(id);


			_db.Entry(od).State = EntityState.Modified;

			try
			{
				var Order = _db.CustomerOrderTable
					.Where(m => m.OrderId == oddetailsdto.OrderId).FirstOrDefault();
				Order.OrderId = oddetailsdto.OrderId;
				Order.OrderDate= oddetailsdto.OrderDate;
				Order.CustomerAddress = oddetailsdto.CustomerAddress;
				Order.AmountAfterDiscount = oddetailsdto.AmountAfterDiscount;

				await _db.SaveChangesAsync();
 			}
			catch (DbUpdateConcurrencyException)
			{
				if (!OrderExists(id))
				{
					return "修改失敗!";
				}
				else
				{
					throw;
				}
			}

			return "修改成功!";
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
