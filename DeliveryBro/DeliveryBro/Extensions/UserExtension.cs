using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace DeliveryBro.Extensions
{
	public static class UserExtension
	{
		public static Guid GetId(this ClaimsPrincipal claimsPrincipal,string role)
		{
			Claim? temp;
           if (role=="Store")
				temp=claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Contains("RestaurantId"));
			else if(role== "Administrator")
                temp=claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Contains("AdministratorId"));
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
}
