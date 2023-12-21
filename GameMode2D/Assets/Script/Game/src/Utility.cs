using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public static class Utility
{
    public static string GetRandomCharAndNumber(int length)
    {
        string result = string.Empty;
        string strPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        int max = strPool.Length - 1;

        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());

        int idx = 0;
        for (int i = 0; i < length; ++i)
        {
            idx = rand.Next(0, max);
            result += strPool[idx];
        }

        return result;
    }

    public static long UtcNowUnixTimeMilliseconds()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }


    public static bool ParsePhoneNumber(string number, string checkRegion)
    {
        var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
        var phoneNumber = phoneNumberUtil.Parse(number, checkRegion);
        bool isValidNumber = phoneNumberUtil.IsValidNumber(phoneNumber); // returns true for valid number  
        string region = phoneNumberUtil.GetRegionCodeForNumber(phoneNumber); // GB, US , PK    
        var numberType = phoneNumberUtil.GetNumberType(phoneNumber); // Produces Mobile , FIXED_LINE    

        var succses = isValidNumber && numberType == PhoneNumbers.PhoneNumberType.MOBILE;
        return succses;
    }

    public static void SetScreenOrientation(bool left, bool right, bool upside, bool sidedown)
    {
        // // 初始狀態, 該狀態可以自由翻轉螢幕, 
        // // 它雖然效果跟 AutoRotation 相同, 
        // // 但值卻不一樣, 所以想要自由翻轉的話, 還是使用 AutoRotation 吧! 
        // Screen.orientation = ScreenOrientation.Unknown;


        // // 螢幕翻轉為 正向, 話筒在上, 且不能翻轉
        // Screen.orientation = ScreenOrientation.Portrait;


        // // 螢幕翻轉為 倒向, 話筒在下, 且不能翻轉
        // Screen.orientation = ScreenOrientation.PortraitUpsideDown;


        // // 螢幕翻轉為 向左倒, 話筒在左, 且不能翻轉, 
        // // 這邊 Landscape 與 LandscapeLeft 意思是一樣的, 
        // // 就連值也相同, 所以兩個都可以使用. 
        // Screen.orientation = ScreenOrientation.LandscapeLeft;


        // // 螢幕翻轉為 向右倒, 話筒在右, 且不能翻轉
        // Screen.orientation = ScreenOrientation.LandscapeRight;


        // 可自由翻轉, 但可以設定方向的限制
        Screen.orientation = ScreenOrientation.AutoRotation;

        // 設定是否可以 向左倒
        Screen.autorotateToLandscapeLeft = left;

        // 設定是否可以 向右倒
        Screen.autorotateToLandscapeRight = right;

        // 設定是否可以 正向
        Screen.autorotateToPortrait = upside;

        // 設定是否可以 倒向
        Screen.autorotateToPortraitUpsideDown = sidedown;
    }

    public static void VisualElementDisplayEnable(VisualElement element, bool enabled)
    {
        if (enabled)
            element.style.display = DisplayStyle.Flex;
        else
            element.style.display = DisplayStyle.None;
    }
}
