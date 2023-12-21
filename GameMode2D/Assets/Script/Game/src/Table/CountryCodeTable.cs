using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryCodeTable : TableBase
{
    private const string s_id = "ID";
    private const string s_name = "Name";
    private const string s_code = "Code";
    private const string s_abbreviation = "Abbreviation";

    private Dictionary<int, CountryCodeInfo> _idCountryCodeInfoTable = new Dictionary<int, CountryCodeInfo>();

    public Dictionary<int, CountryCodeInfo> CountryCodeInfoTable { get => _idCountryCodeInfoTable; }

    public CountryCodeInfo GetCountryCodeInfo(int id)
    {
        _idCountryCodeInfoTable.TryGetValue(id, out CountryCodeInfo countryCodeInfo);
        return countryCodeInfo;
    }

    protected override void OnRowParsed(List<object> rowContent)
    {
        int id = rowContent[GetColumnNameIndex(s_id)] as ValueTypeWrapper<int>;
        string name = rowContent[GetColumnNameIndex(s_name)] as ValueTypeWrapper<string>;
        string code = rowContent[GetColumnNameIndex(s_code)] as ValueTypeWrapper<string>;
        string abbreviation = rowContent[GetColumnNameIndex(s_abbreviation)] as ValueTypeWrapper<string>;

        CountryCodeInfo countryCodeInfo = new()
        {
            ID = id,
            countryName = name,
            countryCode = code,
            countryAbbreviation = abbreviation,
        };

        if (!_idCountryCodeInfoTable.ContainsKey(id))
        {
            _idCountryCodeInfoTable.Add(id, countryCodeInfo);
        }
        else
        {
            _idCountryCodeInfoTable[id] = countryCodeInfo;
            Debug.LogWarningFormat("idCountryCodeInfoTable has have countryCodeInfo for {0} id", id);
        }
    }

    protected override void OnTableParsed()
    {
    }
}


public class CountryCodeInfo
{
    public int ID;
    public string countryName;
    public string countryCode;
    public string countryAbbreviation;
}