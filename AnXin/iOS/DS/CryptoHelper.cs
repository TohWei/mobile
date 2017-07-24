using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Xamarin.Forms;
using PCLAnXin;
using AnXin.iOS;

[assembly: DependencyAttribute(typeof(CryptoHelper))]
namespace AnXin.iOS
{
	public class CryptoHelper : ICryptoHelper
	{
		// For DES Encryption
		private const string KEY = "yKU9kWCYVHtWNVUhcmRagqTwIwfAm5TF";
		private const string IV = "pQgrZ64lMRs=";

		// For DUID Hash Password Check
		private const string Password = "BiI3lg8v4QAXYTimuoqK5AA62STd3RJym3HzQ1XWnf2FZPEnbuCabDfPskgyLER";

		private static readonly Dictionary<char, char> Base64ToURLChar = new Dictionary<char, char>
		{
			{ '+', '$' },
			{ '/', '-' },
			{ '=', '_' }
		};
		
		private static readonly Dictionary<char, char> URLToBase64Char = new Dictionary<char, char>
		{
			{ '$', '+' },
			{ '-', '/' },
			{ '_', '=' }
		};
		
		private static string BytesToBase64URL(byte[] bytes) {
			var base64 = Convert.ToBase64String(bytes);
			char[] base64URL = new char[base64.Length];
			for (int i=0; i<base64.Length; i++) {
				if(Base64ToURLChar.ContainsKey(base64[i])) {
					base64URL[i] = Base64ToURLChar[base64[i]];
				} else {
					base64URL[i] = base64[i];
				}
			}
			return new string(base64URL);
		}
		
		private static byte[] Base64URLToBytes(string base64URL) {
			char[] base64 = new char[base64URL.Length];
			for (int i=0; i<base64URL.Length; i++) {
				if(URLToBase64Char.ContainsKey(base64URL[i])) {
					base64[i] = URLToBase64Char[base64URL[i]];
				} else {
					base64[i] = base64URL[i];
				}
			}
			var base64String = new string(base64);
			return Convert.FromBase64String(base64String);
		}

		// Uses DES for reasonably short Cipher Text (Couldn't use AES with CFB due to bug in mono)
		public static string EncryptText(String Data)
		{
			byte[] keyBytes = Convert.FromBase64String(KEY);
			byte[] IVBytes = Convert.FromBase64String(IV);

			try
			{
				byte[] plainBytes = Encoding.UTF8.GetBytes(Data);

				using (var ms = new MemoryStream()) 
				using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(keyBytes, IVBytes), CryptoStreamMode.Write)) 
				{
					cs.Write(plainBytes, 0, plainBytes.Length);
					cs.FlushFinalBlock();
					return BytesToBase64URL(ms.ToArray());
				}
			}
			catch(CryptographicException e)
			{
				Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
				return null;
			}
		}

		// Uses DES
		public static string DecryptText(String CryptoString)
		{
			byte[] keyBytes = Convert.FromBase64String(KEY);
			byte[] IVBytes = Convert.FromBase64String(IV);

			try
			{
				using(var ms = new MemoryStream(Base64URLToBytes(CryptoString)))
				using(var cs = new CryptoStream(ms, 
					new TripleDESCryptoServiceProvider().CreateDecryptor(keyBytes, IVBytes), 
					CryptoStreamMode.Read))
				using(var sReader = new StreamReader(cs))
				{
					return sReader.ReadLine();
				}
			}
			catch(CryptographicException e)
			{
				Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
				return null;
			}
		}

		// Returns a hash of text using SHA256
		public static string GetHash(string text) {
			var stringBytes = Encoding.UTF8.GetBytes(text+Password);
			var sha = new SHA256Managed();
			var hashBytes = sha.ComputeHash(stringBytes);
			return Convert.ToBase64String(hashBytes);
		}

		//Interfaces can't have static members and static methods can not be used as implementation of interface methods.
		string ICryptoHelper.GetHash(string text)
		{
			return CryptoHelper.GetHash(text);
		}
		string ICryptoHelper.EncryptText(String data)
		{
			return CryptoHelper.EncryptText (data);
		}
		string ICryptoHelper.DecryptText(String data)
		{
			return CryptoHelper.DecryptText(data);
		}
	}
}

