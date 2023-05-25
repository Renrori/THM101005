using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryBro.Models;
using DeliveryBro.ViewModels.Restaurant;
using DeliveryBro.Dao;

namespace DeliveryBro.Controllers
{
	public class RestaurantController : Controller
	{
		private readonly sql8005site4nownetContext _context;

		public RestaurantController(sql8005site4nownetContext context)
		{
			_context = context;
		}

		// GET: Restaurant
		public async Task<IActionResult> Login()
		{
			return View();
			//_context.RestaurantTable != null ?
			//        View(await _context.RestaurantTable.ToListAsync()) :
			//        Problem("Entity set 'sql8005site4nownetContext.RestaurantTable'  is null.");
		}
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public IActionResult Login([Bind("RestaurantAccount,RestaurantPassword")] RestaurantLoginViewModel cvm)
		{
			if (ModelState.IsValid)
			{
				//1.查詢帳號
				RestaurantDao restaurantDao = new RestaurantDao();
				RestaurantTable restaurantTable = restaurantDao.GetUserPassWord(cvm.RestaurantAccount);



				//2.查詢出來的帳號對應到的密碼去比較
				if (cvm.RestaurantPassword == restaurantTable.RestaurantPassword)
				{
					//登入成功
					return View("Views/Home/Index.cshtml");
				}
				else
				{
					//停留原本畫面(資料庫是char)
					return RedirectToAction("Index");
				}

				
			}
			return View(cvm);
		}

		// GET: Restaurant/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.RestaurantTable == null)
			{
				return NotFound();
			}

			var restaurantTable = await _context.RestaurantTable
				.FirstOrDefaultAsync(m => m.RestaurantId == id);
			if (restaurantTable == null)
			{
				return NotFound();
			}

			return View(restaurantTable);
		}

		// GET: Restaurant/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Restaurant/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("RestaurantId,RestaurantAccount,RestaurantPassword,RestaurantName,RestaurantAddress,RestaurantPhone,RestauranEmail,OpeningHours,RestauranDescription,RestaurantPicture")] RestaurantTable restaurantTable)
		{
			if (ModelState.IsValid)
			{
				_context.Add(restaurantTable);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(restaurantTable);
		}

		// GET: Restaurant/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.RestaurantTable == null)
			{
				return NotFound();
			}

			var restaurantTable = await _context.RestaurantTable.FindAsync(id);
			if (restaurantTable == null)
			{
				return NotFound();
			}
			return View(restaurantTable);
		}

		// POST: Restaurant/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("RestaurantId,RestaurantAccount,RestaurantPassword,RestaurantName,RestaurantAddress,RestaurantPhone,RestauranEmail,OpeningHours,RestauranDescription,RestaurantPicture")] RestaurantTable restaurantTable)
		{
			if (id != restaurantTable.RestaurantId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(restaurantTable);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!RestaurantTableExists(restaurantTable.RestaurantId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(restaurantTable);
		}

		// GET: Restaurant/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.RestaurantTable == null)
			{
				return NotFound();
			}

			var restaurantTable = await _context.RestaurantTable
				.FirstOrDefaultAsync(m => m.RestaurantId == id);
			if (restaurantTable == null)
			{
				return NotFound();
			}

			return View(restaurantTable);
		}

		// POST: Restaurant/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.RestaurantTable == null)
			{
				return Problem("Entity set 'sql8005site4nownetContext.RestaurantTable'  is null.");
			}
			var restaurantTable = await _context.RestaurantTable.FindAsync(id);
			if (restaurantTable != null)
			{
				_context.RestaurantTable.Remove(restaurantTable);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool RestaurantTableExists(int id)
		{
			return (_context.RestaurantTable?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
		}
	}
}
