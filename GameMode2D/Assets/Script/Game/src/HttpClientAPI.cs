using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HttpClientAPI : MonoBehaviour
{
    static string s_siteId = "400000015";
    static string s_firmId = "400000602";
    static string s_lang = "cn";
    static string s_currency = "CNY";
    // static string s_domain = "http://mwstg.666wins.com/as-lobby/";

    static string s_MW_publicKey = "<RSAKeyValue><Modulus>spkxZRhQbMWSROjG1oDIkpuUuvFVlkkXJ+gBlVQwTFp7RBRRz/VhQopQTI0JqbDhPBb9i10UuEthhqqXBQYmZfP9uqZCMuRh+dTbBzcQFuzivvkDZG/cKrgKUiFihN4IG1D6vyss53KW/SJ+U6WPu+Ww47swDQ4HNNippPRVI5E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    static string s_EC_privateKey = "<RSAKeyValue><Modulus>vtUWiqP5Mceb/mt21PBUdGYbpKkRGGXUO3m2lAPK8O5Ose/fyYU9IDktFmewzLTtB3+VA3YOAPJlWEOt6C+3Dye9ug92wdKDExMTeJa8QEPVN3SkM6yiqarDRyRfhIaySrbWvkrZT8rgt0YHOnAangC5Bl6m679zBhTqs9R0bbE=</Modulus><Exponent>AQAB</Exponent><P>6GDruMn2PzyBIaN7ZjsGIG1ziP6qN5wy4w00RF9M0Ht8zYAmH3B3OBakcao8N/yhFVjldLZRR7ajZ2DA89Z5GQ==</P><Q>0jsHiLEfPWm64c4IyVZBhu4bKhCsSoJ72gadUYXARMLIIltiqoFUcz1wdcLw5ArhZGMkNtTBDGnzc8M7/810WQ==</Q><DP>pwbTJ9Vyu+0/W/BoCAkw1CoXu0ZhDuuk3/JjuSlOyyOXhxYvULXD23ra5CBafFuHZRKqiwNo1MUAGpQ+3IUyMQ==</DP><DQ>XYo2R/PHWqP4qw/piOwAK/E11PmmL2Dvior25JcGfZHNSrwuon74/G2R5FPgqxbMQsZ6DouLeeKKmC9+OstHwQ==</DQ><InverseQ>u91EEiciwK3x7QqwuDJvDt4b3t19btPSlmCWq1+VBizJoxW1Dsse3ylWQl5FJvvDgXLf/+A/hQn4xxSW+ajb3g==</InverseQ><D>gcBA42M6PC6MUiCfW4lM4xfKE9sgVIZoF0haa6logwiFWVbPwiVlulMl5OX7wDQENeT5XLEYNGybm7fotsY6oFZjcNj9FeALl+55H8RF42QBFE0K+pW9xw91DWkiu7PYP1Vb2LUctSj+ZigMdqODmmPd/7OIa/wcwKN4CUAyhAE=</D></RSAKeyValue>";


    public async Task<string> Oauth(string domain, string uid, int gameId, string lang)
    {
        Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "lang", lang },
                { "uid", uid }, // 区分大小写，总长度必须小于32个字符 (uid、utoken、merchantId 接入方自行生成與管理)
                { "utoken", "testutoken0012345678901234567890" }, // ec平台用户授权码，一次授权之后不可变更，长度必须为32个字符
                { "firmId", s_firmId },
                { "jumpType", "0" },
                { "currency", s_currency },
                { "gameId", gameId }, //要進入的gameId
            };

        string apiUrl = domain + "api/nmw/common/Oauth";
        var response = await sendApi("Oauth", apiUrl, param);
        return response;
    }

    public async Task<string> UserInfo(string domain, string uid)
    {
        Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "uid", uid }, // 区分大小写，总长度必须小于32个字符 (uid、utoken、merchantId 接入方自行生成與管理)
                { "utoken", "testutoken0012345678901234567890" }, // ec平台用户授权码，一次授权之后不可变更，长度必须为32个字符
                { "firmId", s_firmId },
            };

        string apiUrl = domain + "api/nmw/transfer/UserInfo";
        var response = await sendApi("UserInfo", apiUrl, param);
        return response;
    }

    public async Task<string> GameInfo(string domain, string lang)
    {
        Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "lang", lang },
                { "timestamp", Utility.UtcNowUnixTimeMilliseconds() },
            };

        string apiUrl = domain + "api/nmw/common/GameInfo";
        var response = await sendApi("GameInfo", apiUrl, param);
        return response;
    }

    public async Task<string> Transfer(string domain, string uid, long transferAmount)
    {
        Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "lang", s_lang },
                { "uid", uid }, // 区分大小写，总长度必须小于32个字符 (uid、utoken、merchantId 接入方自行生成與管理)
                { "utoken", "testutoken0012345678901234567890" }, // ec平台用户授权码，一次授权之后不可变更，长度必须为32个字符
                { "firmId", s_firmId },
                { "transferType", 0 }, //0 轉入，1 轉出
                { "transferOrderNo", "APP_" + Utility.GetRandomCharAndNumber(16) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") },
                { "transferAmount", transferAmount.ToString() },
                { "transferClientIp", "0.0.0.0" },
                { "currency", s_currency },
            };

        string apiUrl = domain + "api/nmw/transfer/Transfer";
        var response = await sendApi("Transfer", apiUrl, param);
        return response;
    }

    private async Task<string> sendApi(string func, string apiUrl, Dictionary<string, object> param)
    {
        Debug.Log("Send Func: " + func);
        Debug.Log("Send Url: " + apiUrl);

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("method", func);
        client.DefaultRequestHeaders.Add("siteId", s_siteId);

        var keyData = encryptKeyData(param);

        Dictionary<string, object> send = new Dictionary<string, object>();
        send.Add("data", keyData.Item2);
        send.Add("key", keyData.Item1);

        var response = await client.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(send), Encoding.UTF8, "application/json"));
        var content = await response.Content.ReadAsStringAsync();
        Debug.Log("Response: " + content);
        string decryptString = decryptResponse(content);
        Debug.Log("Decrypt: " + decryptString);
        return decryptString;
    }

    private Tuple<string, string> encryptKeyData(Dictionary<string, object> param)
    {
        // 生成AESKEY 長度16
        string aesKey = Utility.GetRandomCharAndNumber(16);

        // 用 MW平台公鑰 加密 AESKEY 獲得 加密KEY
        string encryptKey = RSAHelper.RSAEncrypt(s_MW_publicKey, aesKey);

        // 把 DATA 轉成 JSON字串
        string jsonString = JsonConvert.SerializeObject(param);

        // 用 AESKEY 加密 JSON字串 獲得 加密DATA
        string encryptData = AESHelper.AESEncrypt(aesKey, jsonString);

        Debug.Log("Send Key: " + encryptKey);
        Debug.Log("Send Data: " + encryptData);

        return new Tuple<string, string>(encryptKey, encryptData);
    }

    private string decryptResponse(string response)
    {
        JObject json = JsonConvert.DeserializeObject<JObject>(response);

        // 用 EC平台私鑰 解密 加密KEY 獲得 AESKEY
        string aesKey = RSAHelper.RSADecrypt(s_EC_privateKey, json["key"].ToString());

        // 用 AESKEY 解密 加密DATA 獲得 JSON字串
        return AESHelper.AESDecrypt(aesKey, json["data"].ToString());
    }
}