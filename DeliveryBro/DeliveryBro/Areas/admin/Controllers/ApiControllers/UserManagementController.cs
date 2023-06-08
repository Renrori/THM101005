//using DeliveryBro.Areas.admin.Models.ViewModel;
//using DeliveryBro.Areas.store.DTO;
//using DeliveryBro.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace DeliveryBro.Areas.admin.Controllers.ApiControllers
//{

//	[Route("api/UserManagement/[action]")]
//	[ApiController]
//	public class UserManagementController : ControllerBase
//	{
//		private readonly sql8005site4nownetContext _db;
//		public UserManagementController(sql8005site4nownetContext context)
//		{
//			_db = context;
//		}

//		[HttpGet]
//		public async Task<UserManagementDTO> GetUser()
//		{
//			var User = await _db.CustomersTable
//			 .Select(User => new UserManagementDTO
//			 {
//				 CustomerAccount = User.CustomerAccount,
//				 CustomerName = User.CustomerName,
//				 CustomerEmail = User.CustomerEmail,
//				 CustomerPhone = User.CustomerPhone,
//			 }).ToListAsync();


//			return User;
//		}

//		[HttpPost]
//		public IEnumerable<UserManagementDTO> FilterLevel(
//		 [FromBody] UserManagementDTO urViewModel)
//		{
//			return _db.CustomerAccount.Where(le =>
//			le.LevelId == urViewModel.CustomerAccount ||
//			le.Name.Contains(urViewModel.Name))
//			.Select(le => new UserManagementDTO
//			{
//				CustomerAccount = User.CustomerAccount,
//				CustomerName = User.CustomerName,

//			});

//		}

//		[HttpPut]
//		public async Task<string> PutLevel(int id, [FromBody] UserManagementDTO urViewModel)
//		{
//			if (id != urViewModel.CustomerName)
//			{
//				return "修改會員失敗!";
//			}
//			CustomersTable ur = await _db.CustomersTable.FindAsync(id);

//			_db.Entry(ur).State = EntityState.Modified;

//			try
//			{
//				await _db.SaveChangesAsync();
//			}
//			catch (DbUpdateConcurrencyException)
//			{
//				if (!UserExists(id))
//				{
//					return "修改會員失敗!";
//				}
//				else
//				{
//					throw;
//				}
//			}

//			return "修改會員成功!";
//		}
//		private bool UserExists(int id)
//		{
//			return (_db.CustomersTable?.Any(e => e.CustomerAccount == id)).GetValueOrDefault();
//		}

//	}
//	[HttpPost]
//	public async Task<string> PostLevel([FromBody] LevelViewModel leViewModel)
//	{
//		Level le = new Level
//		{
//			LevelId = leViewModel.LevelId,
//			Name = leViewModel.Name,
//		};
//		_db.Levels.Add(le);
//		await _context.SaveChangesAsync();

//		return $"會員編號:{le.LevelId}";
//	}

//	[HttpDelete]
//	public async Task<string> Delete(int id)
//	{

//		var = await _db.Levels.FindAsync(id);
//		if (Levels == null)
//		{
//			return "刪除會員成功!";
//		}

//		_db.Levels.Remove(Levels);
//		try
//		{
//			await _db.SaveChangesAsync();
//		}
//		catch (DbUpdateException ex)
//		{
//			return "刪除會員失敗!";
//		}

//		return "刪除會員成功!";
//	}

//	private bool LevelExists(int id)
//	{
//		return (_db.Levels?.Any(e => e.LevelId == id)).GetValueOrDefault();
//	}
//}
//	}
//}
//	}
