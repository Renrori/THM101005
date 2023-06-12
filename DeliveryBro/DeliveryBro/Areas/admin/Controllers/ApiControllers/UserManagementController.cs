using DeliveryBro.Areas.admin.Models.ViewModel;
using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace DeliveryBro.Areas.admin.Controllers.ApiControllers
{

	[Route("api/UserManagement/[action]")]
	[ApiController]
	public class UserManagementController : ControllerBase
	{
		private readonly sql8005site4nownetContext _db;
		public UserManagementController(sql8005site4nownetContext context)
		{
			_db = context;
		}
		//IEnumerable 副數集合方法

		/// <summary>
		/// 讀取USER資料
		/// </summary>
		/// <param name = "page" > 頁數 </ param >
		/// < returns > 回傳DTP </ returns >
		[HttpGet("{page}")]
		public async Task<UserManagementDTO> GetUser(int page)
		{
			var quantity = 10;
			var count = 0;
			var query = _db.CustomersTable
			.OrderBy(x => x.CustomerAccount);
			count = await query.CountAsync();

			var User = await query
			.Skip(quantity * (page - 1))
			.Take(quantity)
			.Select(User => new UserDTO
			{
				CustomerAccount = User.CustomerAccount,
				CustomerName = User.CustomerName,
				CustomerEmail = User.CustomerEmail,
				CustomerPhone = User.CustomerPhone,
			}).ToListAsync();

			return new UserManagementDTO
			{
				Count = count,
				Items = User
			};
		}


		[HttpPut]
		public async Task<string> PutUser(string id, [FromBody] UserDTO usdetailsdto)
		{
			if (id != usdetailsdto.CustomerAccount)
			{
				return "修改失敗!";
			}
			CustomersTable od = await _db.CustomersTable.FindAsync(id);

			_db.Entry(od).State = EntityState.Modified;

			try
			{
				await _db.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(id))
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


		[HttpDelete("{Id}")]
		public async Task<string> Delete(string Id)
		{ 
			var key= _db.CustomersTable.FirstOrDefault(x=>x.CustomerAccount==Id).CustomerId;
			var r =await _db.CustomersTable.FindAsync(key);
			if (r == null)
			{
				return "找不到資料";
			}

			//foreach (var item in r.CustomersTable)
			//{
			//	_db.CustomersTable.Remove(item);
			//}

			_db.CustomersTable.Remove(r);
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
		private bool UserExists(string id)
		{
			return (_db.CustomersTable?.Any(e => e.CustomerAccount == id)).GetValueOrDefault();
		}

		[HttpPost]
		public async Task<string> CreateUser([FromBody] UserDTO user)
		{
			CustomersTable newuser = new CustomersTable
			{
				CustomerAccount = user.CustomerAccount,
				CustomerName = user.CustomerName,
				CustomerEmail = user.CustomerEmail,
				CustomerPhone = user.CustomerPhone,
				CustomerPassword = user.CustomerPassword,
			};
			_db.CustomersTable.Add(newuser);
			await _db.SaveChangesAsync();
			return "新增成功";
		}
	}
}