using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Data;
using DeliveryBro.Extensions;
using DeliveryBro.Models;
using DeliveryBro.ViewModels.Home;
using DeliveryBro.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;

namespace DeliveryBro.ApiController
{
    [ApiExplorerSettings(IgnoreApi = true)]
    //[EnableCors("User")]  限制跨域來源
    
    [Route("api/[controller]")]
    [Authorize(Roles = "User", AuthenticationSchemes = "CustomerAuthenticationScheme")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public UserApiController(sql8005site4nownetContext context)
        {
            _context = context;
        }

        //GET: api/UserApi/23
        [HttpGet("{customerId:guid}")]
        public async Task<IActionResult> GetUserInfo(Guid customerId)
        {
            // 從資料庫中查找對應的用戶記錄
            var user = await _context.CustomersTable.FindAsync(customerId);

            if (user == null)
            {
                return NotFound(); // 如果找不到用戶記錄，返回 NotFound 結果
            }

            var userAddress = await _context.CustomerAddressTable.Where(c => c.CustomerId == customerId).Select(c => c.CustomerAddress).ToListAsync();

            var userInfo = new UserInfoViewModel
            {
                UserId = user.CustomerId,
                UserName = user.CustomerName,
                UserEmail = user.CustomerEmail,
                UserAddress = userAddress,
                UserBirth = user.DateOfBirth,
                UserPhone = user.CustomerPhone,
                UserPicture = user.CustomerPhoto
            };

            return Ok(userInfo);
        }


        //Put:api/UserApi/4
        [HttpPut("{customerId:guid}")]
        public async Task<string> EditUserInfo(Guid customerId, EditUserInfoViewModel eui)
        {
            //Httpcontext
            CustomersTable userInfo = await _context.CustomersTable.FindAsync(customerId);

            userInfo.CustomerName = eui.UserName;
            userInfo.CustomerEmail = eui.UserEmail;
            userInfo.DateOfBirth = eui.UserBirth;
            userInfo.CustomerPhone = eui.UserPhone;
            //userInfo.CustomerPhoto = eui.UserPicture;

            //track追蹤改變者上傳
            _context.Entry(userInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerId))
                {
                    return "修改失敗"!;
                }
                else
                {
                    throw;
                }
            }
            return "成功";

        }

        [Route("CheckAdd/{customerId:guid}")]
        [HttpPost]
        public async Task<IActionResult> CheckAddress(Guid customerId , UserAddressViewModel address)
        {
            var userAddCount = _context.CustomerAddressTable.Where(x=>x.CustomerId == customerId).Count();

            if(userAddCount == 0)
            {
                CustomerAddressTable userAdd = new CustomerAddressTable
                {
                    CustomerAddress = address.UserAddress,
                    CustomerId = customerId
                };
                _context.CustomerAddressTable.Add(userAdd);
                await _context.SaveChangesAsync();

                return Ok("新增地址成功");
            }

           CustomerAddressTable userAddtar = _context.CustomerAddressTable.FirstOrDefault(x => x.CustomerId == customerId);
            userAddtar.CustomerAddress = address.UserAddress;

            _context.Entry(userAddtar).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("修改地址成功");
        }

        [Route("hisorder")]
        public async Task<IEnumerable<UserOrderViewModel>> GetUserOrder()
        {
            var id = User.GetId();
            var orderDetails = _context.CustomerOrderTable
            .Where(o => o.CustomerId == id && o.OrderStatus == "completed").OrderByDescending(x => x.OrderId).Select(o => new UserOrderViewModel
            {

                OrderId = o.OrderId,
                OrderDate = o.OrderDate.ToLocalTime().ToString(),
                CustomerName = o.Customer.CustomerName,
                Note = o.Note,
                OrderDetails = o.OrderDetailsTable.Select(d => new UserOrderDetailsViewModel
                {
                    DishName = d.DishName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal
                }).ToList(),
                Total = o.OrderDetailsTable.Sum(o => o.Subtotal)
            });

            return orderDetails;
        }

        [HttpGet("waitorder")]
        public  IEnumerable<UserOrderViewModel> GetWaitOrder ()
        {
			var id = User.GetId();
            CheckOrder(id);
			var orderDetails = _context.CustomerOrderTable
                .Where(o => o.CustomerId == id && (o.OrderStatus == "waiting" || o.OrderStatus == "acepted")).OrderByDescending(x => x.OrderId).Select(o => new UserOrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.ToLocalTime().ToString(),
                    CustomerName = o.Customer.CustomerName,
                    Note = o.Note,
                    OrderDetails = o.OrderDetailsTable.Select(d => new UserOrderDetailsViewModel
                    {
                        DishName = d.DishName,
                        UnitPrice = d.UnitPrice,
                        Quantity = d.Quantity,
                        Discount = d.Discount,
                        Subtotal = d.Subtotal
                    }).ToList(),
                    Total = o.OrderDetailsTable.Sum(o => o.Subtotal)
                });

            return orderDetails;
        }

        private void CheckOrder(Guid id)
        {
            DateTime dt = DateTime.Now;
            _context.CustomerOrderTable.Where(o => o.CustomerId == id && o.OrderStatus == "waiting").ForEachAsync(
                x =>
                {
                    TimeSpan ts = dt - x.OrderDate;
                    if(ts.Minutes > 10)
                    {
                        x.OrderStatus = "refused";
                    }
                }
                );


        }

        [HttpPost("pic/{customerId}")]
        public async Task<IActionResult> PostUserPic(Guid customerId,IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("請選擇有效的圖片檔案");
            }

            byte[] photoBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                photoBytes = memoryStream.ToArray();
            }

            CustomersTable customerPic = await _context.CustomersTable.FindAsync(customerId);
            if (customerPic == null)
            {
                return NotFound("找不到指定的客戶");
            }
            customerPic.CustomerPhoto = photoBytes;

            await _context.SaveChangesAsync();

            return Ok("上傳成功");
        }
        private bool CustomerExists(Guid customerId)
        {
            return (_context.CustomersTable?.Any(e => e.CustomerId == customerId)).GetValueOrDefault();
        }

        //-----------原本的方法-----------
        //[Authorize]
        //GET:api/UserApi/2
        //[HttpGet("{customerId}")]
        //[Authorize(Role = "User")] 限制登入者?
        //public async Task<UserInfoViewModel> GetUserInfo(int customerId)
        //{
        //var login = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //if (login.Succeeded)
        //{
        //    var userIdClaim = login.Principal.Claims.FirstOrDefault(c => c.ValueType == "customerId");
        //    var userId = userIdClaim?.Value;
        //    var user = await _context.CustomersTable.Include(c => c.CustomerAddressTable)
        //    .FirstOrDefaultAsync(c => c.CustomerId == userId);
        //}

        //    var userAddress = await _context.CustomerAddressTable.Where(c => c.CustomerId == customerId).Select(c => c.CustomerAddress).ToListAsync();
        //    var user = await _context.CustomersTable.FindAsync(customerId);

        //    UserInfoViewModel userInfo = new UserInfoViewModel
        //    {

        //        UserId = user.CustomerId,
        //        UserName = user.CustomerName,
        //        UserEmail = user.CustomerEmail,
        //        UserAddress = userAddress,
        //        UserBirth = user.DateOfBirth,
        //        UserPhone = user.CustomerPhone,
        //        UserPicture = user.CustomerPhoto

        //    };
        //    return userInfo;
        //}
        //-------------------------------
    }
}
