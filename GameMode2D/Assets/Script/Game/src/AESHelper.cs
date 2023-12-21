using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

public class AESHelper
{
    public static string AESEncrypt(string key, string content)
    {
        AesManaged tdes = new AesManaged();
        tdes.Key = Encoding.UTF8.GetBytes(key);
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;
        ICryptoTransform crypt = tdes.CreateEncryptor();
        byte[] plain = Encoding.UTF8.GetBytes(content);
        byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
        return Convert.ToBase64String(cipher);
    }

    public static string AESDecrypt(string key, string data)
    {
        AesManaged tdes = new AesManaged();
        tdes.Key = Encoding.UTF8.GetBytes(key);
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;
        ICryptoTransform crypt = tdes.CreateDecryptor();
        byte[] plain = Convert.FromBase64String(data);
        byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
        return Encoding.UTF8.GetString(cipher);
    }
}
