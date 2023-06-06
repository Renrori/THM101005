using DeliveryBro.Models;
using DeliveryBro.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public HomeApiController(sql8005site4nownetContext context)
        {
            _context = context;
        }

        //抓取有商品的店家回傳
        [HttpGet]
        // Get:/api/HomeApi/
        public async Task<IEnumerable<StoreViewModel>> GetStore()
        {
            var groupMenu = await _context.MenuTable
               //.Where(m => !string.IsNullOrEmpty(m.DishName))
               .GroupBy(m => m.RestaurantId)
               .Select(g => g.Key)
               .ToListAsync();

            var sm = await _context.RestaurantTable
                .Where(s => groupMenu.Contains(s.RestaurantId))
                .Select(s => new StoreViewModel
                {
                    StoreId = s.RestaurantId,
                    StoreName = s.RestaurantName,
                    StoreAddress = s.RestaurantAddress,
                    StoreDescription = s.RestaurantDescription
                })
                .ToListAsync();
            return sm;
        }
        //Post:/api/HomeApi/Filter
        //此功能未成功請勿使用
        [HttpPost("Filter")]
        public async Task<IQueryable<StoreViewModel>> FilterStore(
            [FromBody]StoreViewModel svm) 
        {
            
            var groupMenu = await _context.MenuTable
               .GroupBy(m => m.RestaurantId)
               .Select(g => g.Key)
               .ToListAsync();
            
            var sm = _context.RestaurantTable
                .Where(s => groupMenu.Contains(s.RestaurantId));

                
            if(svm.StoreName != null) 
            {
                sm = sm.Where(s => s.RestaurantName.Contains(svm.StoreName));
                    
            }
            return sm.Select(s => new StoreViewModel
            {
                StoreId = s.RestaurantId,
                StoreName = s.RestaurantName,
                StoreAddress = s.RestaurantAddress,
                StoreDescription = s.RestaurantDescription
            });     
     }



        
        [HttpGet("{id:guid}")]
        // Get:/api/HomeApi/1
        //之後改成傳物件呼叫形式
        public async Task<IEnumerable<MenuViewModel>> GetProduct(Guid id)
        {
            
            var product = await _context.MenuTable.Include(m => m.Restaurant)
               .Where(m => m.Restaurant.RestaurantId == id && m.DishStatus == "ongoing")
               .Select(p => new MenuViewModel
               {
                   DishId = p.DishId,
                   RestaurantId = p.RestaurantId,
                   DishName = p.DishName,
                   DishPrice = p.DishPrice,
                   DishPicture = p.DishPicture,
                   DishDescription = p.DishDescription,
                   DishCategory = p.DishCategory
               })
               .ToArrayAsync();
            return product;
        }

        [HttpPost("{restaurantId}/Filter")]
        public async Task<IQueryable<MenuViewModel>> FilterStore(
            [FromBody]MenuViewModel mvm) 
        {
            var product = await _context.MenuTable.Include(m => m.RestaurantId)
                .Where(m => m.Restaurant.RestaurantId == mvm.RestaurantId && m.DishStatus == "ongoing")
                .ToArrayAsync();

            var pd = product.Where(p =>
            p.DishName.Contains(mvm.DishName) ||
            p.DishPrice.ToString().Contains(mvm.DishPrice.ToString()))
                .Select(p => new MenuViewModel
                {
                    DishId = p.DishId,
                    RestaurantId = p.RestaurantId,
                    DishName = p.DishName,
                    DishPrice = p.DishPrice,
                    DishPicture = p.DishPicture,
                    DishDescription = p.DishDescription,
                    DishCategory = p.DishCategory
                }) ;
            return pd.AsQueryable();
        }

        [HttpGet("getpic/{storeId:guid}")]
        // Get:/api/HomeApi/getpic/1
        //叫用商店圖片方法，傳入StoreId回傳圖片
        public async Task<IActionResult> GetPictureStore(Guid storeId)
        {
            RestaurantTable c = await _context.RestaurantTable.FindAsync(storeId);
            byte[] imgUrl = c?.RestaurantPicture;

            if (c == null || c.RestaurantPicture == null)
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "noimgmed.png");
                byte[] fakeImageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                return File(fakeImageBytes, "image/png");
            }

            return File(imgUrl, "img/jpeg");

        }

        [HttpPost]
        //Post:/api/HomeApi/
        public async Task<IActionResult> GetOrder([FromBody] OrderViewModel order)
        {
            if(order == null)
            {
                return BadRequest();
            }
            try
            {
                CustomerOrderTable cot = new CustomerOrderTable
                {
                    OrderId = order.OrderId,
                    CustomerAddress = order.CustomerAddress,
                    ShippingFee = order.ShippingFee,
                    Payment = order.Payment,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = order.OrderStatus,
                    Note = order.Note,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                };
                _context.CustomerOrderTable.Add(cot);
                await _context.SaveChangesAsync();

                int orderId = cot.OrderId; //儲存訂單的自動識別ID
                
                foreach (var od in order.OrderDetailViewModels)
                {
                    od.OrderId = orderId;
                    var checkOd = _context.MenuTable.Include(m => m.Restaurant)
                        .Where(m => m.Restaurant.RestaurantId == order.RestaurantId && m.DishStatus == "ongoing")
                        .FirstOrDefault(d => d.DishId == od.DishId); //搜尋對應的資料庫商品

                    if(checkOd == null) 
                    {
                        return BadRequest("訂單商品已有異動，請重新確認");
                    }

                    OrderDetailsTable odt = new OrderDetailsTable  //打印訂單Entity
                    {
                        OrderId = od.OrderId,
                        DishId = od.DishId,
                        OrderDate= DateTime.UtcNow.Date,
                        UnitPrice = checkOd.DishPrice,
                        Quantity = od.Quantity,
                        DishName = checkOd.DishName,
                    };
                    odt.Subtotal = odt.UnitPrice * odt.Quantity;
                    _context.OrderDetailsTable.Add(odt);
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest("訂單上傳失敗");
            }
            return Ok("訂單上傳成功");
        }

        [HttpPost("NewebPay")]
        //Post:/api/HomeApi/
        public async Task<IActionResult> CreateNPOrder([FromBody] OrderViewModel order)
        {
            int Total = 0;
            if (order == null)
            {
                return BadRequest();
            }
            try
            {
                CustomerOrderTable cot = new CustomerOrderTable
                {
                    OrderId = order.OrderId,
                    CustomerAddress = order.CustomerAddress,
                    ShippingFee = order.ShippingFee,
                    Payment = order.Payment,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = order.OrderStatus,
                    Note = order.Note,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                };
                _context.CustomerOrderTable.Add(cot);
                await _context.SaveChangesAsync();

                int orderId = cot.OrderId; //儲存訂單的自動識別ID

                foreach (var od in order.OrderDetailViewModels)
                {
                    od.OrderId = orderId;
                    var checkOd = _context.MenuTable.Include(m => m.Restaurant)
                        .Where(m => m.Restaurant.RestaurantId == order.RestaurantId && m.DishStatus == "ongoing")
                        .FirstOrDefault(d => d.DishId == od.DishId); //搜尋對應的資料庫商品

                    if (checkOd == null)
                    {
                        return BadRequest("訂單商品已有異動，請重新確認");
                    }

                    OrderDetailsTable odt = new OrderDetailsTable  //打印訂單Entity
                    {
                        OrderId = od.OrderId,
                        DishId = od.DishId,
                        OrderDate = DateTime.UtcNow.Date,
                        UnitPrice = checkOd.DishPrice,
                        Quantity = od.Quantity,
                        DishName = checkOd.DishName,
                    };
                    odt.Subtotal = odt.UnitPrice * odt.Quantity;
                    Total += odt.Subtotal;
                    _context.OrderDetailsTable.Add(odt);
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest("訂單上傳失敗");
            }


            NewebPayViewModel npOrder = new NewebPayViewModel
            {
                OrderId = order.OrderId,
                OrderTotal = Total,
                PayCardType = "CREDIT"

            };

            return Ok(npOrder);
        }
    }
}
