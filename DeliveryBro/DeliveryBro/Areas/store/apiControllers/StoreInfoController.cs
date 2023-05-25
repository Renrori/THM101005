﻿using DeliveryBro.Areas.store.DTO;
using DeliveryBro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DeliveryBro.Areas.store.apiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreInfoController : ControllerBase
    {
        private readonly sql8005site4nownetContext _context;

        public StoreInfoController(sql8005site4nownetContext context)
        {
            _context = context;
        }

        // GET: api/RestaurantTables
        [HttpGet]
        public async Task<IQueryable<StoreInfoDTO>> GetRestaurantTable()
        {
            return _context.RestaurantTable.Where(x => x.RestaurantId == 3).Select(x => new StoreInfoDTO
            {
                RestaurantId = x.RestaurantId,
                RestaurantDescription = x.RestaurantDescription,
                RestaurantEmail = x.RestaurantEmail,
                RestaurantName = x.RestaurantName,
                RestaurantAddress = x.RestaurantAddress,
                RestaurantPhone = x.RestaurantPhone,
                RestaurantPicture = x.RestaurantPicture,
                OpeningHours = x.OpeningHours
            });
        }

        private async Task<byte[]> Getpic(IFormFile file)
        {
            if (file != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }
        private async Task SetPic(RestaurantTable store, IFormFile file)
        {
            store.RestaurantPicture = await Getpic(file);
        }
        // PUT: api/RestaurantTables/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutRestaurantTable(int id, IFormCollection form)
        {

            RestaurantTable store = await _context.RestaurantTable.FindAsync(id);
            store.RestaurantDescription = form["RestaurantDescription"];
            DateTime dateTime = DateTime.ParseExact(form["openingHours"], "HH:mm", CultureInfo.InvariantCulture);
            store.OpeningHours = dateTime.TimeOfDay;
            IFormFile picfile = form.Files.GetFile("RestaurantPicture");
            if (picfile != null)
                await SetPic(store, picfile);
            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTableExists(id))
                {
                    return "無法更改";
                }
                else
                {
                    throw;
                }
            }

            return "更改成功";
        }



        private bool RestaurantTableExists(int id)
        {
            return (_context.RestaurantTable?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }
    }
}
