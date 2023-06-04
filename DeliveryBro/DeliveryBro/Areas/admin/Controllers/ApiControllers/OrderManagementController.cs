using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.Areas.admin.Controllers.ApiControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderManagementController : ControllerBase
    {
        private readonly sql8005site4nownetContext _db;
        public OrderManagementController(sql8005site4nownetContext context)
        {
            _db = context;
        }

        ///api/OrderManagement/All <summary>
        [Route("all")]
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var r = await _db.CustomerOrderTable.Select(x => new
            {
                OrderID = x.OrderId,
                OrderDate = x.OrderDate.ToUniversalTime().ToLocalTime().ToString(),
                CustomerAdd = x.CustomerAddress,
                AmountAfterDiscount = x.AmountAfterDiscount,
                OrderStatus = x.OrderStatus,
            }).ToListAsync();

            return Ok(r);
        }
    }
}
