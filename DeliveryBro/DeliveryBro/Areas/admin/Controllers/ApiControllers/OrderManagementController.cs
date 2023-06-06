﻿using DeliveryBro.Areas.admin.Models.ViewModel;
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
        [HttpGet]
        public async Task<IEnumerable<OrderManagementDTO>> GetOrder()
        {
            var Order = await _db.CustomerOrderTable
             .Select(Order => new OrderManagementDTO
             {
                 OrderId = Order.OrderId,
                 OrderDate = Order.OrderDate,
                 OrderStatus = Order.OrderStatus,
                 AmountAfterDiscount = Order.AmountAfterDiscount,
                 CustomerAddress = Order.CustomerAddress
             }).ToListAsync();

            return Order;
        }
        [HttpPut]
        public async Task<string> PutOrder(int id, [FromBody] OrderManagementDTO oddetailsdto)
        {
            if (id != oddetailsdto.OrderId)
            {
                return "修改失敗!";
            }
            CustomerOrderTable od = await _db.CustomerOrderTable.FindAsync(id);


            _db.Entry(od).State = EntityState.Modified;

            try
            {
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
            var r = await _db.CustomerOrderTable.Include(o=> o.OrderDetailsTable).Where(x=> x.OrderId == Id).FirstOrDefaultAsync();
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

        ///api/OrderManagement/All <summary>

     
    
   private bool OrderExists(int id)
    {
        return (_db.CustomerOrderTable ?.Any(e => e.OrderId == id)).GetValueOrDefault();
    }
    
    }
}
