//using DeliveryBro.Areas.store.DTO;
//using DeliveryBro.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace DeliveryBro.Areas.admin.Controllers.ApiControllers
//{
//    [Route("api/UserManagement/[action]")]
//    [ApiController]
//    public class UserManagementController : ControllerBase
//    {
//        private readonly sql8005site4nownetContext _db;
//        public UserManagementController(sql8005site4nownetContext context)
//        {
//            _db = context;
//        }
//    }

//}

////[HttpGet]
////public async Task<IEnumerable<LevelViewModel>> GetLevel()
////{
////    var Level = await _context.Levels
////     .Select(Level => new LevelViewModel
////     {
////         LevelId = Level.LevelId,
////         Name = Level.Name

////     })
////     .ToListAsync();

////    return Level;
////}

//[HttpPost]
//public async Task<IEnumerable<LevelViewModel>> FilterLevel(
// [FromBody] LevelViewModel leViewModel)
//{
//    return _context.Levels.Where(le =>
//    le.LevelId == leViewModel.LevelId ||
//    le.Name.Contains(leViewModel.Name))
//    .Select(le => new LevelViewModel
//    {
//        LevelId = le.LevelId,
//        Name = le.Name

//    });

//}

//[HttpPut]
//public async Task<string> PutLevel(int id, [FromBody] LevelViewModel leViewModel)
//{
//    if (id != leViewModel.LevelId)
//    {
//        return "修改會員等級失敗!";
//    }
//    Level le = await _context.Levels.FindAsync(id);
//    le.LevelId = leViewModel.LevelId;
//    le.Name = leViewModel.Name;

//    _context.Entry(le).State = EntityState.Modified;

//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateConcurrencyException)
//    {
//        if (!LevelExists(id))
//        {
//            return "修改會員等級失敗!";
//        }
//        else
//        {
//            throw;
//        }
//    }

//    return "修改會員等級成功!";
//}
//[HttpPost]
//public async Task<string> PostLevel([FromBody] LevelViewModel leViewModel)
//{
//    Level le = new Level
//    {
//        LevelId = leViewModel.LevelId,
//        Name = leViewModel.Name,
//    };
//    _context.Levels.Add(le);
//    await _context.SaveChangesAsync();

//    return $"會員等級編號:{le.LevelId}";
//}

//[HttpDelete]
//public async Task<string> DeleteLevel(int id)
//{

//    var Levels = await _context.Levels.FindAsync(id);
//    if (Levels == null)
//    {
//        return "刪除會員等級成功!";
//    }

//    _context.Levels.Remove(Levels);
//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateException ex)
//    {
//        return "刪除會員等級失敗!";
//    }

//    return "刪除會員等級成功!";
//}

//private bool LevelExists(int id)
//{
//    return (_context.Levels?.Any(e => e.LevelId == id)).GetValueOrDefault();
//}
//    }
//}
