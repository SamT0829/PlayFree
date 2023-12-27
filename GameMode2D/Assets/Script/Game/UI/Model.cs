using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class GameInfoData
{
    public List<GameData> games;
}

public class GameData
{
    public int gameId;
    public string gameName;
    public string gameIcon;
    public string gameIcon2;
}

public class UserInfoData
{
    public string balance;
}

public class GameWebURLData
{
    public string interface2;
}

public class TransferData
{
    public string transferAmount;
    public string beginMoney;
    public string endMoney;
}


[Serializable]
public class ThemeSettings
{
    public string themeName;
    public ThemeStyleSheet tss;
}

public class CountryCodeInfo
{
    public int ID;
    public string countryName;
    public string countryCode;
    public string countryAbbreviation;
}