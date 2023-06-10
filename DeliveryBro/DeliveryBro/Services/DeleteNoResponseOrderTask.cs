using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.Services
{
	public class DeleteNoResponseOrderTask
	{
		private readonly sql8005site4nownetContext _context;
		public DeleteNoResponseOrderTask(sql8005site4nownetContext context)
		{
			_context = context;
		}
		public async Task<NoResponseOrderDTO> DeleteOrder()
		{
			var query = _context.CustomerOrderTable
				.Where(x => x.OrderDate.AddMinutes(3) > DateTime.Now)
				.Select(x => new NoResponseOrderDTO
				{
					Id=x.OrderId,
					OrderDate=x.OrderDate.AddMinutes(3),
				});
			foreach(var order in query) 
			{
				var delorder = _context.CustomerOrderTable.FindAsync(order.Id);
				var deldetail= _context.OrderDetailsTable.Where(x=>x.OrderId==order.Id).ToArrayAsync();
				_context.RemoveRange(deldetail);
				_context.Remove(deldetail);
				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateException)
				{
					return null;
				}
			}
			return null;
			
		}
	}
}
