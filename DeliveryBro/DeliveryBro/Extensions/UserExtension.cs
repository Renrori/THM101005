using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace DeliveryBro.Extensions
{
	public static class UserExtension
	{
		public static int GetId(this ClaimsPrincipal claimsPrincipal)
		{
			var temp = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Contains("RestaurantId"));
			if (temp == null) return -1;
			return int.Parse(temp.Value);
		}
	}
}
