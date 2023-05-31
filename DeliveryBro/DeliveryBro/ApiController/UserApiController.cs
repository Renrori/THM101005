using DeliveryBro.Models;
using DeliveryBro.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.ApiController
{
    //[EnableCors("User")]  限制跨域來源
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public UserApiController(sql8005site4nownetContext context)
        {
            _context = context;
        }

        //[Authorize]
        ////GET:api/UserApi/2
        //[HttpGet]
        ////[Authorize(Role="User")] 限制登入者?
        //public async Task<UserInfoViewModel> GetUserInfo()
        //{
        //    var login = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    if (login.Succeeded)
        //    {
        //        var userIdClaim = login.Principal.Claims.FirstOrDefault(c=>c.ValueType== "customerId");
        //        var userId = userIdClaim?.Value;
        //        var user = await _context.CustomersTable.Include(c => c.CustomerAddressTable)
        //        .FirstOrDefaultAsync(c => c.CustomerId == userId);
        //    }
            

        //    var userAddress = await _context.CustomerAddressTable.Where(c => c.CustomerId == customerId).Select(c => c.CustomerAddress).ToListAsync();

        //    var userInfo = new UserInfoViewModel
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

        //Put:api/UserApi/4
        [HttpPut("{customerId}")]
        public async Task<string> EditUserInfo(int customerId, EditUserInfoViewModel eui)
        {
            

            //Httpcontext
            CustomersTable userInfo = await _context.CustomersTable.FindAsync(customerId);

            userInfo.CustomerName = eui.UserName;
            userInfo.CustomerEmail = eui.UserEmail;
            userInfo.DateOfBirth = eui.UserBirth;
            userInfo.CustomerPhone = eui.UserPhone;
            userInfo.CustomerPhoto = eui.UserPicture;

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

        private bool CustomerExists(int customerId)
        {
            return (_context.CustomersTable?.Any(e => e.CustomerId == customerId)).GetValueOrDefault();
        }
    }
}
