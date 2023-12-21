using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginScreen : MonoBehaviour
{
    public static Action OnClickLanguageButton;

    [SerializeField] private UIDocument m_Document;
    [SerializeField] private VisualTreeAsset m_PhoneCountry;

    private const string s_loginScreenName = "login-screen";
    private const string s_loginUserNameTextFieldName = "login-username-textfield";
    private const string s_loginUserNameIconName = "login-username-icon";
    private const string s_loginPhoneNumberTextFieldName = "login-phonenumber-textfield";
    private const string s_loginPhoneNumberIconName = "login-phonenumber-icon";
    private const string s_loginPhoneNumberWrongMessageIconName = "login-phonenumber_wrong-message-icon";
    private const string s_unityTextInputName = "unity-text-input";

    private const string s_loginLanguageButtonName = "login-language-button";

    private const string s_loginConfirmButtomName = "login-confirm-button";

    private const string s_loginCountryCodeName = "login-country-code";
    private const string s_loginCountryCodeLabelName = "login-country-code_label";
    private const string s_loginCountryCodeScrollViewName = "login-country-code_scrollview";


    private bool m_WrongMessage = false;
    private bool m_loginCountryShow = false;

    private VisualElement m_root;
    private VisualElement m_loginScreen;
    private TextField m_userNameTextField;
    private VisualElement m_userNameIcon;
    private TextField m_phoneNumberTextField;
    private VisualElement m_phoneNumberMessageIcon;
    private VisualElement m_phoneNumberWrongMessageIcon;
    private Button m_loginLanguageButton;
    private Button m_confirmButton;

    private VisualElement m_loginCountryCode;
    private Label m_loginCountryLabel;
    private ScrollView m_loginCountryScrollView;

    private List<PhoneCountryCode> m_phoneCountryCodes = new();

    private void Awake()
    {
        m_root = m_Document.rootVisualElement;

        SetVisualElements();
        RegisterButtonCallbacks();

        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Utility.SetScreenOrientation(true, true, false, false);
    }

    private void Start()
    {
        if (GameManager.Instance.PhoneNumberData.PhoneNumber != string.Empty)
            GameManager.Instance.SendLoginRequest(GameManager.Instance.PhoneNumberData.UserName, GameManager.Instance.PhoneNumberData.PhoneNumber);

        CreateTable();
    }

    public void CreateTable()
    {
        TableManager.Instance.CreateTable<CountryCodeTable>(nameof(CountryCodeTable));
        SetCountryCodeSetting();
    }

    private void OnEnable()
    {
        GameManager.AccountLoginFinish += () => SetLoginScreenEnable(false);
    }

    private void OnDisable()
    {
        GameManager.AccountLoginFinish -= () => SetLoginScreenEnable(false);
    }

    private void SetVisualElements()
    {
        m_loginScreen = m_root.Q(s_loginScreenName);
        m_userNameTextField = m_root.Q<TextField>(s_loginUserNameTextFieldName);
        m_userNameIcon = m_root.Q(s_loginUserNameIconName);
        m_phoneNumberTextField = m_root.Q<TextField>(s_loginPhoneNumberTextFieldName);
        m_phoneNumberMessageIcon = m_root.Q(s_loginPhoneNumberIconName);
        m_phoneNumberWrongMessageIcon = m_root.Q(s_loginPhoneNumberWrongMessageIconName);
        m_confirmButton = m_root.Q<Button>(s_loginConfirmButtomName);
        m_loginLanguageButton = m_root.Q<Button>(s_loginLanguageButtonName);

        m_loginCountryCode = m_root.Q(s_loginCountryCodeName);
        m_loginCountryLabel = m_root.Q<Label>(s_loginCountryCodeLabelName);
        m_loginCountryScrollView = m_root.Q<ScrollView>(s_loginCountryCodeScrollViewName);
    }

    private void RegisterButtonCallbacks()
    {
        m_confirmButton.RegisterCallback<ClickEvent>(OnClickConfirm);
        m_loginLanguageButton.RegisterCallback<ClickEvent>(e => OnClickLanguageButton.Invoke());
        m_phoneNumberTextField.RegisterCallback<ClickEvent>(e =>
        {
            m_WrongMessage = false;
            Utility.VisualElementDisplayEnable(m_phoneNumberWrongMessageIcon, false);
        });

        m_loginCountryCode.RegisterCallback<ClickEvent>(e =>
        {
            if (m_loginCountryShow)
            {
                m_loginCountryShow = false;
                Utility.VisualElementDisplayEnable(m_loginCountryScrollView, false);
            }
            else
            {
                m_loginCountryShow = true;
                Utility.VisualElementDisplayEnable(m_loginCountryScrollView, true);
            }
        });

        m_userNameTextField.RegisterCallback<FocusEvent>(e =>
        {
            OnTextFieldIconEnable(m_userNameTextField, m_userNameIcon, false);
        });

        m_userNameTextField.RegisterCallback<FocusOutEvent>(e =>
        {
            OnTextFieldIconEnable(m_userNameTextField, m_userNameIcon, true);
        });

        m_phoneNumberTextField.RegisterCallback<FocusEvent>(e =>
        {
            OnTextFieldIconEnable(m_phoneNumberTextField, m_phoneNumberMessageIcon, false);
        });

        m_phoneNumberTextField.RegisterCallback<FocusOutEvent>(e =>
        {
            OnTextFieldIconEnable(m_phoneNumberTextField, m_phoneNumberMessageIcon, !m_WrongMessage);
        });
    }

    private void SetCountryCodeSetting()
    {
        var table = TableManager.Instance.GetTable<CountryCodeTable>().CountryCodeInfoTable;
        var iter = table.Values.ToList();

        for (int i = 0; i < iter.Count; i++)
        {
            VisualElement element = m_PhoneCountry.Instantiate();

            PhoneCountryCode phoneCountryCode = new();
            phoneCountryCode.SetVisualElements(element);
            phoneCountryCode.InitCountryCode(iter[i]);
            m_phoneCountryCodes.Add(phoneCountryCode);

            m_loginCountryScrollView.Q("unity-content-container").Add(element);

            element.RegisterCallback<ClickEvent>(e =>
            {
                Utility.VisualElementDisplayEnable(m_loginCountryScrollView, false);
                m_loginCountryLabel.text = phoneCountryCode.CountryCodeNumber;
                GameManager.Instance.PhoneRegion = phoneCountryCode.CountryAbbreviation;
            });
        }
    }

    private void OnTextFieldIconEnable(TextField textField, VisualElement textFieldIcon, bool show = true)
    {
        if (textField == null || textFieldIcon == null)
            return;

        if (textField.text == string.Empty && show)
            textFieldIcon.style.display = DisplayStyle.Flex;
        else
            textFieldIcon.style.display = DisplayStyle.None;
    }

    private void OnClickConfirm(ClickEvent evt)
    {
        if (GameManager.Instance.WaitRespond())
            return;

        var phoneNumber = m_loginCountryLabel.text + m_phoneNumberTextField.value;
        var userName = m_userNameTextField.value;

        try
        {
            var isValidNumber = Utility.ParsePhoneNumber(phoneNumber, GameManager.Instance.PhoneRegion);

            if (isValidNumber)
            {
                Debug.Log("The mobile phone number is a valid number");
                GameManager.Instance.PhoneNumberData.UserName = userName;
                GameManager.Instance.PhoneNumberData.PhoneNumber = phoneNumber;
                GameManager.Instance.SendLoginRequest(userName, phoneNumber);
            }
            else
            {
                Debug.Log("The mobile phone number is a invalid number");
                Utility.VisualElementDisplayEnable(m_phoneNumberWrongMessageIcon, true);
                m_phoneNumberTextField.value = "";
                m_WrongMessage = true;
            }
        }
        catch
        {
            Debug.Log("The mobile phone number is a failed number");
            Utility.VisualElementDisplayEnable(m_phoneNumberWrongMessageIcon, true);
            m_phoneNumberTextField.value = "";
            m_WrongMessage = true;
        }
    }

    private void SetLoginScreenEnable(bool enabled)
    {
        if (enabled)
            m_loginScreen.style.display = DisplayStyle.Flex;
        else
            m_loginScreen.style.display = DisplayStyle.None;
    }
}
