using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpDelete]
        public async Task<string> DeleteOrderManagement(int id)
        {
            var r = await _db.CustomerOrderTable.FindAsync(id);
            if (r == null)
            {
                return "刪除成功";
            }

            _db.CustomerOrderTable.Remove(r);
            try
            {
                  await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return "刪除失敗!";
            }

            return "刪除成功!";
        }

        ///api/OrderManagement/All <summary>

        [HttpGet]
        public object All()
        {
            var r =  _db.CustomerOrderTable.Select(r => new
            {
                OrderID = r.OrderId,
                OrderDate = r.OrderDate.ToUniversalTime().ToLocalTime().ToString(),
                CustomerAdd = r.CustomerAddress,
                AmountAfterDiscount = r.AmountAfterDiscount,
                OrderStatus = r.OrderStatus,
            }).ToList();

            return Ok(r);
        }
    }
    public class DeleteDto { 
           public int ID { get; set; } 
    }
}
