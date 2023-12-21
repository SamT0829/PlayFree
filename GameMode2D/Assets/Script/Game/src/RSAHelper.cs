using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

public class RSAHelper
{
    public static string RSAEncrypt(string key, string content)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
        rsa.FromXmlString(key);
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] textBytes = encoding.GetBytes(content);
        byte[] encryptedOutput = rsa.Encrypt(textBytes, false);
        return Convert.ToBase64String(encryptedOutput);
    }

    public static string RSADecrypt(string key, string content)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(key);
        byte[] plainbytes = rsa.Decrypt(Convert.FromBase64String(content), false);
        return Encoding.UTF8.GetString(plainbytes);
    }
}
