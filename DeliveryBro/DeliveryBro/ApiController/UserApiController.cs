using DeliveryBro.Models;
using DeliveryBro.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryBro.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public UserApiController(sql8005site4nownetContext context)
        {
            _context = context;
        }


        //GET:api/UserApi/2
        [HttpGet("{customerId}")]
        public async Task<UserInfoViewModel> GetUserInfo(int customerId)
        {
            var user = await _context.CustomersTable.FindAsync(customerId);
            UserInfoViewModel userInfo = new UserInfoViewModel
            {
                UserId =user.CustomerId,
                UserName = user.CustomerName,
                UserEmail = user.CustomerEmail,
                UserBirth = user.DateOfBirth,
                UserPhone =user.CustomerPhone,
                UserPicture = user.CustomerPhoto
     
            };
            return userInfo;
        }

        //[HttpPost("{customerId}")]
        //public async Task<string> EditUserInfo(int customerId, EditUserInfoViewModel eui)
        //{
        //    if (customerId != eui.UserId)
        //    {
        //        return "資料不符合";
        //    }
        //    //Httpcontext
        //    CustomersTable cus = await _context.CustomersTable.FindAsync(customerId);
        //    {
               
        //    }
        //}
    }
}
