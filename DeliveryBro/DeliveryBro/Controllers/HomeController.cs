using DeliveryBro.Models;
using DeliveryBro.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DeliveryBro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly sql8005site4nownetContext _context;

        public HomeController(ILogger<HomeController> logger , sql8005site4nownetContext context)
        {
            _logger = logger;
            _context = context;
        }

        //預設回傳View
        public IActionResult Index()
        {
            return View();
        }
        
        //抓取有商品的店家回傳店家資訊模型(Id,Name)
        public async Task<IActionResult> GetStore()
        {
            var groupMenu = await _context.MenuTable
               //.Where(m => !string.IsNullOrEmpty(m.DishName))
               .GroupBy(m => m.DishId)
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
            return Ok(sm);
        }

        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.MenuTable.Include(m => m.Restaurant)
               .Where(m => m.Restaurant.RestaurantId == id && m.DishStatus == "ongoing" )
               .Select(p => new MenuViewModel 
               {
                   DishId = p.DishId,
                   RestaurantId = p.RestaurantId,
                   DishName = p.DishName,
                   DishPrice = p.DishPrice,
                   DishDescription = p.DishDescription,
                   DishCategory = p.DishCategory
               })
               .ToArrayAsync();
            return Ok(product);
        }

 
        //叫用商店圖片方法，傳入StoreId回傳圖片
        public async Task<IActionResult> GetPictureStore(int storeId)
        {
            RestaurantTable c = await _context.RestaurantTable.FindAsync(storeId);
            byte[] imgUrl = c?.RestaurantPicture;
            return File(imgUrl, "img/jpeg");
           
        }
        //叫用商品圖片方法，傳入storeId和dishId回傳圖
        public async Task<FileResult> GetPicture(CallViewModel call)
        {

            MenuTable c = await _context.MenuTable.Include(m => m.Restaurant)
                .FirstOrDefaultAsync(m => m.RestaurantId == call.storeId && m.DishId == call.dishId);

            byte[] imgUrl = c?.DishPicture;
            return File(imgUrl, "img/jpeg");
        }

        //嘗試傳商店id回傳所有對應圖片
        //public async Task<IActionResult> GetPictures(int id)
        //{
        //    List<FileResult> imageList = new List<FileResult>();

        //    IEnumerable<MenuTable> menuPic = await _context.MenuTable.Where(m => m.RestaurantId == id).ToListAsync();

        //    foreach (var p in menuPic)
        //    {
        //        var imgUrl = File(p.DishPicture, "image/jpeg");
        //        imgUrl.FileDownloadName = Url.Action("GetPictures", "Home", new { storeId = id });
        //        imageList.Add(imgUrl);
        //    }

        //    return new JsonResult(imageList);
        //}

        public IActionResult RestaurantMenu()
        {
            return View();
        }

        public IActionResult OrderList()
        {
            return View();
        }

        public IActionResult CheckoutList()
        {
            return View();
        }
        public IActionResult PaybyCreditCard()
        {
            return View();
        }

        public IActionResult FinalOrderList()
        {
            return View();
        }

        [HttpPost]
        public async Task<bool> GetOrder([FromBody] OrderViewModel order)
        {
            try
            {
                CustomerOrderTable cot = new CustomerOrderTable
                {
                    OrderId = order.OrderId,
                    CustomerAddress = order.CustomerAddress,
                    ShippingFee = order.ShippingFee,
                    OrderDate = order.OrderDate,
                    OrderStatus = order.OrderStatus,
                    Note = order.Note,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                };
                _context.CustomerOrderTable.Add(cot);
                await _context.SaveChangesAsync();

                int orderId = cot.OrderId;

                foreach (var od in order.OrderDetailViewModels)
                {
                    OrderDetailsTable odt = new OrderDetailsTable
                    {
                        OrderId = orderId,
                        DishId = od.DishId,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Subtotal = od.Subtotal,
                        Total = od.Total,
                    };
                    _context.OrderDetailsTable.Add(odt);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}