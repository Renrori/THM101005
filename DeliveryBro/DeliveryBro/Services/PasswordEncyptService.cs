using System.Security.Cryptography;
using System.Text;

namespace DeliveryBro.Services
{
	public class PasswordEncyptService
	{
		internal string PasswordEncrypt(string password)
		{
			byte[] bytesPassword = Encoding.UTF8.GetBytes(password);
			using (var alg = SHA512.Create())
			{
				string hex = "";
				var hashValue = alg.ComputeHash(bytesPassword);
				foreach (byte x in hashValue)
				{
					hex += String.Format("{0:x2}", x);
				}
				return hex;
			}
		}
	}
}
