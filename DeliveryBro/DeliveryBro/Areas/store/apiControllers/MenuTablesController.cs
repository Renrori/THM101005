using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeliveryBro.Models;
using DeliveryBro.Areas.store.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DeliveryBro.Extensions;
using System.IO.Compression;

namespace DeliveryBro.Areas.store.apiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Store", AuthenticationSchemes = "StoreAuthenticationScheme")]
	public class MenuTablesController : ControllerBase
	{
		private readonly sql8005site4nownetContext _context;
		private readonly IWebHostEnvironment _environment;
		public MenuTablesController(sql8005site4nownetContext context,IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}
		
		// GET: api/MenuTables
		[HttpGet]
		public async Task<IQueryable<MenuDTO>> GetMenuTables()
		{
			var id = User.GetId();
			var query = _context.MenuTable.AsNoTracking().Where(x => x.RestaurantId == id).Select(menu => new MenuDTO
			{
				DishId = menu.DishId,
				DishCategory = menu.DishCategory,
				DishDescription = menu.DishDescription,
				DishName = menu.DishName,
				DishPrice = menu.DishPrice,
				DishStatus = menu.DishStatus,
				PictureUrl=menu.PicturePath
			});

			return query;
		}

		[HttpPut("status/{id}")]
		public async Task<string> ChangeDishStatus(int id, MenuStatusDTO msDTO)
		{
			if (id != msDTO.DishId)
			{
				return "狀態修改失敗";
			}
			MenuTable menu = await _context.MenuTable.FindAsync(id);
			menu.DishStatus = msDTO.DishStatus;
			_context.Entry(menu).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MenuTableExists(id))
					return "狀態修改失敗";
				else
					throw;
			}
			return "狀態修改成功";
		}
		// PUT: api/MenuTables/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<string> PutMenuTable(int id, IFormCollection form)
		{
			if (id != int.Parse(form["DishId"]))
			{
				return "品項修改失敗";
			};
			var rid = User.GetId();
			MenuTable menu = await _context.MenuTable.FindAsync(id);
			menu.DishId = int.Parse(form["DishId"]);
			menu.DishName = form["DishName"];
			menu.DishPrice = int.Parse(form["DishPrice"]);
			menu.DishDescription = form["DishDescription"];
			menu.DishCategory = form["DishCategory"];
			menu.DishStatus = form["DishStatus"];
			menu.RestaurantId = rid;
			IFormFile picfile = form.Files.GetFile("PictureUrl");
			if (picfile != null)
				await SetPic(menu, picfile);

			_context.Entry(menu).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MenuTableExists(id))
				{
					return "品項修改失敗";
				}
				else
				{
					throw;
				}
			}

			return "品項修改成功";
		}

		private async Task<string> GetPicPath(IFormFile file)
		{
			var root = $@"{_environment.WebRootPath}\store\images";
			var path = $@"{root}\{file.FileName}";
			using(var st=System.IO.File.Create(path))
			{
				file.CopyTo(st);
			}
			return $@"/store/images/{file.FileName}";
		}
		private async Task<byte[]> GetPic(IFormFile file)
		{
			if (file != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await file.CopyToAsync(memoryStream);
					return memoryStream.ToArray();
				}
			}
			return null;
		}
		private async Task SetPic(MenuTable menu, IFormFile file)
		{
			//var getPicTask = GetPic(file);
			//var getPicPathTask = GetPicPath(file);
			//await Task.WhenAll(getPicTask, getPicPathTask);
			//menu.DishPicture = await getPicTask;
			//menu.PicturePath = await getPicPathTask;
			menu.DishPicture = await GetPic(file);
			menu.PicturePath = await GetPicPath(file);
		}

		// POST: api/MenuTables
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<string> PostMenuTable(IFormCollection form)
		{
			var id = User.GetId();
			MenuTable menu = new MenuTable
			{
				DishId = int.Parse(form["DishId"]),
				DishName = form["DishName"],
				DishPrice = int.Parse(form["DishPrice"]),
				DishDescription = form["DishDescription"],
				DishCategory = form["DishCategory"],
				DishStatus = form["DishStatus"],
				RestaurantId = id
			};
			IFormFile picfile = form.Files.GetFile("PictureUrl");
			await SetPic(menu, picfile);
			_context.MenuTable.Add(menu);
			await _context.SaveChangesAsync();
			return $"{menu.DishName} 新增成功";
		}

		// DELETE: api/MenuTables/5
		[HttpDelete("{id}")]
		public async Task<string> DeleteMenuTable(int id)
		{

			var menuTable = await _context.MenuTable.FindAsync(id);
			if (menuTable == null)
			{
				return "品項刪除成功";
			}

			_context.MenuTable.Remove(menuTable);
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

		private bool MenuTableExists(int id)
		{
			return (_context.MenuTable?.Any(e => e.DishId == id)).GetValueOrDefault();
		}
	}
}
