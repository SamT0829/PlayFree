using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour
{
    public static Action LoadingStart;
    public static Action<int> LoadingProcces;

    public static Action AccountLoginFinish;
    public static Action<UserInfoData, GameInfoData> LoginRespond;
    public static Action<UserInfoData> UpdateUserInfoData;

    public static Action OnSwithUI;

    public const string s_region = "TW";
    public const string s_domain = "http://mwstg.666wins.com/as-lobby/";
    private const string s_resourcePath = "GameData/PhoneNumber/PhoneNumber";

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [HideInInspector] public UniWebViewController UniWebViewController;
    [HideInInspector] public HttpClientAPI HttpClientAPI;
    [HideInInspector] public PhoneNumberSO PhoneNumberData;

    [HideInInspector] public string UserName;
    [HideInInspector] public string PhoneNumber;
    public string PhoneRegion = s_region;
    [HideInInspector] public string Language = "en";
    [HideInInspector] public string switchUI = "W";
    public int width = 0;
    public int hight = 0;


    [SerializeField] List<ThemeSettings> m_ThemeSettings;
    [SerializeField] UIDocument m_Document;

    [SerializeField] private bool m_waitRespond;
    [SerializeField] private bool m_LoginSuccses;


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            _instance = this;


        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);

        UniWebViewController = GetComponent<UniWebViewController>();
        HttpClientAPI = GetComponent<HttpClientAPI>();

        if (PhoneNumberData == null)
            PhoneNumberData = Resources.Load<PhoneNumberSO>(s_resourcePath);

        LobbyScreen.LoginRespondFinish += LoginSuccses;


        Debug.Log("screen width = " + Screen.width + " , screen hight = " + Screen.height);

        if (Screen.width > Screen.height)
        {
            width = Screen.width;
            hight = Screen.height;
        }
        else
        {
            width = Screen.height;
            hight = Screen.width;
        }
    }

    private void Start()
    {
        ApplyTheme(Language);
    }

    private void Update()
    {
        CheckIfSwitchUI();
    }

    private void OnEnable()
    {
        LobbyScreen.LoginRespondFinish += LoginSuccses;
        LobbyScreen.OnClickLanguageConfirmButton += () => ApplyTheme(Language);
    }

    private void OnDisable()
    {
        LobbyScreen.LoginRespondFinish -= LoginSuccses;
        LobbyScreen.OnClickLanguageConfirmButton -= () => ApplyTheme(Language);
    }

    public bool WaitRespond()
    {
        return m_waitRespond || m_LoginSuccses;
    }

    public void SendLoginRequest(string userName, string phoneNumber)
    {
        UserName = userName;
        PhoneNumber = phoneNumber;
        StartCoroutine(AccountLogin());
    }

    public IEnumerator SendAccountInfoRequest()
    {

        var response = string.Empty;
        Task.Run(async () => response = await HttpClientAPI.UserInfo(s_domain, PhoneNumber));
        LoadingProcces.Invoke(20);

        while (response == string.Empty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Account Login UserInfo response = " + response);
        LoadingProcces.Invoke(50);
        UserInfoData userInfodata = JsonConvert.DeserializeObject<UserInfoData>(response);

        UpdateUserInfoData.Invoke(userInfodata);
        LoadingProcces.Invoke(100);
    }

    public List<string> GetGameLanguage()
    {
        List<string> language = new();
        m_ThemeSettings.ForEach(theme => language.Add(theme.themeName));

        return language;
    }

    private IEnumerator AccountLogin()
    {
        LoadingStart.Invoke();

        m_waitRespond = true;
        var response = string.Empty;
        Task.Run(async () => response = await HttpClientAPI.Oauth(s_domain, PhoneNumber, 6002, Language));

        LoadingProcces.Invoke(10);

        while (response == string.Empty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Account Login Oauth response = " + response);
        LoadingProcces.Invoke(20);

        response = string.Empty;
        Task.Run(async () => response = await HttpClientAPI.UserInfo(s_domain, PhoneNumber));

        while (response == string.Empty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Account Login UserInfo response = " + response);
        LoadingProcces.Invoke(20);
        UserInfoData userInfodata = JsonConvert.DeserializeObject<UserInfoData>(response);

        response = string.Empty;
        Task.Run(async () => response = await HttpClientAPI.GameInfo(s_domain, Language));

        while (response == string.Empty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        LoadingProcces.Invoke(30);

        Debug.Log("Account Login GameInfo response = " + response);
        GameInfoData gameInfoData = JsonConvert.DeserializeObject<GameInfoData>(response);
        LoginRespond.Invoke(userInfodata, gameInfoData);

        LoadingProcces.Invoke(50);


        while (m_waitRespond)
        {
            yield return new WaitForSeconds(0.1f);
        }

        LoadingProcces.Invoke(90);

        yield return new WaitForSeconds(1f);

        LoadingProcces.Invoke(100);
        AccountLoginFinish.Invoke();
    }

    private void LoginSuccses()
    {
        m_waitRespond = false;
        m_LoginSuccses = true;
    }

    private ThemeStyleSheet GetThemeStyleSheet(string themeName)
    {
        ThemeSettings thisStyleSheet = m_ThemeSettings.Find(x => x.themeName == themeName);

        return thisStyleSheet?.tss;
    }

    private void ApplyTheme(string theme)
    {
        ThemeStyleSheet tss = GetThemeStyleSheet(theme);

        if (tss != null && m_Document != null && m_Document.panelSettings != null)
            m_Document.panelSettings.themeStyleSheet = tss;
    }

    private void CheckIfSwitchUI()
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            if (switchUI == "V")
                return;

            switchUI = "V";
        }

        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            if (switchUI == "W")
                return;

            switchUI = "W";
        }

        OnSwithUI.Invoke();

        Debug.Log("screen width = " + Screen.width + " , screen hight = " + Screen.height);
    }
}
