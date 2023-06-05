using DeliveryBro.Hubs;
using DeliveryBro.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace DeliveryBro.Extensions
{
	public static class UserExtension
	{
		public static UserInfo GetInfo(this ClaimsPrincipal principal)
		{
			var result = new UserInfo();
			foreach (var claim in principal.Claims)
			{
				switch (claim.Type) 
				{
					case "Id":
						result.UserId = new Guid(claim.Value);
						break;
					case ClaimTypes.Name:
						result.Name = claim.Value;
						break;
					case ClaimTypes.Role:
						result.Role = claim.Value;
						break;
				}
			}
			return result;
		}
		public static Guid GetId(this ClaimsPrincipal claimsPrincipal,string role)
		{
			Claim? temp;
           if (role=="Store")
				temp=claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Contains("Id"));
			else if(role== "Administrator")
                temp=claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Contains("Id"));
			else return Guid.Empty;
			return Guid.Parse(temp.Value);
		}
		public static string GetRole(this ClaimsPrincipal claimsPrincipal)
		{
			var role =claimsPrincipal.FindFirst(ClaimTypes.Role).Value;
			if (role == null) return "";
			return role.ToString();
		}
		public static string GetName(this ClaimsPrincipal claimsPrincipal)
		{
			var name = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.Name).Value;
			if (name == null) return "";
			return name;
		}
	}
	public static class ConcurrentDictionaryExtensions
	{
		public static void AutoAddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TValue : class
		{
			dictionary.AddOrUpdate(key, value, (existingKey, existingValue) =>
			{
				if (existingValue != null)
				{
					// 更新現有值的所有欄位
					foreach (var property in typeof(TValue).GetProperties())
					{
						var newValue = property.GetValue(value);
						property.SetValue(existingValue, newValue);
					}
				}

				return existingValue ?? value;
			});
		}
	}
}
