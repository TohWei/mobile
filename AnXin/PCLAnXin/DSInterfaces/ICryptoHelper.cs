using System;

namespace PCLAnXin
{
	public interface ICryptoHelper
	{
		string EncryptText(String Data);
		string GetHash(string text);
		string DecryptText(String CryptoString);
	}
}

