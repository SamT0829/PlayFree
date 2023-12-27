using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class PhoneCountryCode
{
    private const string s_countryCodeName = "country-code";
    private const string s_countryNameLabelName = "country-name-label";
    private const string s_countryCodeNumbererLabelName = "code-number-label";

    public int CountryId;
    public string CountryName = string.Empty;
    public string CountryCodeNumber = string.Empty;
    /// <summary>
    /// 國家名稱縮寫
    /// </summary>
    public string CountryAbbreviation = string.Empty;

    public VisualElement Root;
    public VisualElement m_countryCodeElement;
    private Label m_countryNameLabel;
    private Label m_countryCodeNumberLabel;

    // Init
    public void SetVisualElements(VisualElement root)
    {
        Root = root;
        m_countryCodeElement = Root.Q(s_countryCodeName);
        m_countryNameLabel = Root.Q<Label>(s_countryNameLabelName);
        m_countryCodeNumberLabel = Root.Q<Label>(s_countryCodeNumbererLabelName);
    }
    public void InitCountryCode(CountryCodeInfo countryCodeInfo)
    {
        CountryId = countryCodeInfo.ID;
        CountryName = countryCodeInfo.countryName;
        CountryCodeNumber = countryCodeInfo.countryCode;
        CountryAbbreviation = countryCodeInfo.countryAbbreviation;

        m_countryNameLabel.text = CountryName;
        m_countryCodeNumberLabel.text = CountryCodeNumber;
    }
}
