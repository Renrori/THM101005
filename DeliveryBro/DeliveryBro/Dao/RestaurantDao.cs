using DeliveryBro.Data;
using DeliveryBro.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DeliveryBro.Dao
{
	public class RestaurantDao
	{
		private sql8005site4nownetContext SqlContext;
		public RestaurantDao()
		{
			SqlContext = new sql8005site4nownetContext();
		}

		public RestaurantTable GetUserPassWord(string account)
		{
			RestaurantTable result = SqlContext.RestaurantTable.FromSqlRaw($"select * from Restaurant_Table where RestaurantAccount='{account}' ").ToList().FirstOrDefault();
	
			return result;

		}
	}
}
