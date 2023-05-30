using System.Security.Cryptography;
using System.Text;

namespace DeliveryBro.Services
{
	public class EncryptService
	{
		private readonly IConfiguration configuration;
		private readonly string key;

		public EncryptService(IConfiguration configuration)
		{
			this.key = configuration.GetSection("AesKey").Value;
		}
		public string AesEncryptToBase64(string str)
		{
			using Aes aes = Aes.Create();
			aes.Key = Encoding.UTF8.GetBytes(key);
			aes.GenerateIV();
			byte[] iv = aes.IV;
			using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
			using (var memoryStream = new MemoryStream())
			{
				memoryStream.Write(iv, 0, iv.Length);
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					byte[] plainBytes = Encoding.UTF8.GetBytes(str);
					cryptoStream.Write(plainBytes, 0, plainBytes.Length);
					cryptoStream.FlushFinalBlock();
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		public string AesDecryptToString(string str)
		{
			using Aes aes = Aes.Create();
			aes.Key = Encoding.UTF8.GetBytes(key);
			byte[] encryptedData = Convert.FromBase64String(str);
			string plainText;
			byte[] iv = new byte[aes.IV.Length];
			Array.Copy(encryptedData, 0, iv, 0, iv.Length);

			using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(encryptedData, iv.Length, encryptedData.Length - iv.Length);
					cryptoStream.FlushFinalBlock();
				}
				byte[] decryptedBytes = memoryStream.ToArray();
				plainText = Encoding.UTF8.GetString(decryptedBytes);
			}
			return plainText;
		}
	}
}
