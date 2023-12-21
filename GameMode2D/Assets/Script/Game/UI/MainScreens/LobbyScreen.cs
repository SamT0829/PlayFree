using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyScreen : MonoBehaviour
{
    public static Action<GameWebURLData> OnOpenWebView;
    public static Action LoginRespondFinish;
    public static Action OnClickLanguageConfirmButton;

    public static Action TransferBegin;
    public static Action TransferFinish;

    [SerializeField] protected UIDocument m_Document;
    [SerializeField] VisualTreeAsset m_gameInfoContainer;
    [SerializeField] VisualTreeAsset m_gameInfo;

    [SerializeField] private long m_transferMoney;
    [SerializeField] private bool m_waitRespond;


    // name
    private const string s_lobbyScreenName = "lobby-screen";
    private const string s_lobbyGameInfoPanelName = "lobby-gameinfo-panel";
    private const string s_lobbyGameInfoScrollViewName = "lobby-gameinfo_scrollview";
    private const string s_lobbyGameInfoScrollViewContainerName = "unity-content-container";

    private const string s_lobbyPageName = "lobby-page";
    private const string s_lobbyMenuPanelName = "lobby-menu-panel";
    private const string s_lobbyInformationPanelName = "language-screen";

    private const string s_lobbyGameInfoRightButtonName = "lobby-gameinfo_right-button";
    private const string s_lobbyGameInfoLeftButtonName = "lobby-gameinfo_left-button";

    private const string s_lobbyTransferTextFieldName = "lobby-transfer-textfield";
    private const string s_lobbyTransferTotalMoneyLabelName = "lobby-transer-total-money";

    private const string s_lobbyTransferAddButtonName = "lobby-transfer_add-button";

    private const string s_lobbyInformationUserLabelName = "lobby-information_user-label";
    private const string s_lobbyInformationMenuButtonName = "lobby-information_menu-button";
    private const string s_lobbyInformationMenuButtonDownName = "lobby-information_menu-button-down";

    private const string s_lobbyMenuLanguageButtonName = "lobby-menu_language-button";

    private const string s_languageElementName = "lang-";
    private const string s_languageSelectionElemenName = "lang-selected";

    private const string s_lobbyLanguagesCloseButtonName = "lobby-languages_close-button";
    private const string s_lobbyLanguagesConfirmButtonName = "lobby-languages_confim-button";

    // class
    private const string s_gameInfoContainerClass = "game-info-container";
    private const string s_gameInfoContainerScrollClass = "game-info-container_scroll";

    private const string s_gameInfoContainerMoveLeftClass = "game-info-container_move-left";
    private const string s_gameInfoContainerMoveRightClass = "game-info-container_move-right";

    private const string s_maxSizeClass = "max-size";
    private const string s_lobbyPageBlackClass = "lobby-page-black";
    private const string s_lobbyPageWhiteClass = "lobby-page-white";

    // panel
    private VisualElement m_root;
    private VisualElement m_lobbyScreen;
    private VisualElement m_lobbyGameInfoPanelParent;
    private ScrollView m_lobbyGameInfoScrollView;
    private VisualElement m_lobbyGameInfoScrollViewContainer;

    private VisualElement m_lobbyPageParent;
    private VisualElement m_lobbyMenuPanel;
    private VisualElement m_LanguagesPanel;


    private Button m_lobbyGameInfoLeftButton;
    private Button m_lobbyGameInfoRightButton;

    private TextField m_lobbyTransferTextField;
    private Label m_lobbyTransferTotalMoneyLabel;
    private Button m_lobbyTransferAddButton;

    private Label m_lobbyInformationUserLabel;
    private Button m_lobbyInformationMenuButton;
    private Button m_lobbyInformationMenuDownButton;

    private Button m_LobbyMenuLanguageButton;

    private Button m_LobbyLanguagesCloseButton;
    private Button m_lobbyLanguagesConfirmButton;

    private VisualElement m_langSelectedElement;
    private List<VisualElement> m_languagesElemant = new();

    private List<GameInfoContainer> m_gameInfoContainers = new();

    private int m_lobbyPageTotalCount = 0;
    private float ScrollViewConfirmWidth = 0;
    [SerializeField] private int m_lobbyCurrentPage = 0;
    private string m_choseLanguage = string.Empty;

    private bool m_onTranferMoney = false;

    private void Awake()
    {
        m_root = m_Document.rootVisualElement;

        SetVisualElements();
        RegisterButtonCallbacks();
        SetLanguagesSetting();

        m_lobbyPageTotalCount = 0;
        m_lobbyTransferTextField.value = "0";
        m_choseLanguage = GameManager.Instance.Language;
    }

    private void OnEnable()
    {
        GameManager.LoginRespond += (userInfodata, gamInfoData) => StartCoroutine(GenerateLobbyInfo(userInfodata, gamInfoData));
        GameManager.AccountLoginFinish += () => SetLobbyScreenEnable(true);
        GameManager.UpdateUserInfoData += OnUpdateUserInfoData;

        GameScreen.OnCloseWebView += OnCloseWebView;

        LoginScreen.OnClickLanguageButton += () => LanguagesPanelEnable(true);
    }

    private void OnDisable()
    {
        GameManager.LoginRespond -= (userInfodata, gamInfoData) => StartCoroutine(GenerateLobbyInfo(userInfodata, gamInfoData));
        GameManager.AccountLoginFinish -= () => SetLobbyScreenEnable(true);
        GameManager.UpdateUserInfoData -= OnUpdateUserInfoData;

        GameScreen.OnCloseWebView -= OnCloseWebView;

        LoginScreen.OnClickLanguageButton += () => LanguagesPanelEnable(true);
    }

    private void Update()
    {
        if (m_gameInfoContainers.Count > 0 && ScrollViewConfirmWidth == 0)
        {
            var info = m_gameInfoContainers.First();
            var width = info.Root.resolvedStyle.width;
            if (!Single.IsNaN(width))
            {
                ScrollViewConfirmWidth = width;
            }
        }

        if (ScrollViewConfirmWidth != 0)
        {
            var locationx = Mathf.Abs(m_lobbyGameInfoScrollViewContainer.Q("unity-content-container").resolvedStyle.translate.x) + ScrollViewConfirmWidth / 2;
            var page = (int)Mathf.Floor(locationx / ScrollViewConfirmWidth);
            m_lobbyCurrentPage = page;
            CurruntPageSetting();
        }
    }

    public void SetVisualElements()
    {
        m_lobbyScreen = m_root.Q(s_lobbyScreenName);

        m_lobbyGameInfoPanelParent = m_root.Q(s_lobbyGameInfoPanelName);
        m_lobbyGameInfoScrollView = m_root.Q<ScrollView>(s_lobbyGameInfoScrollViewName);
        m_lobbyGameInfoScrollViewContainer = m_root.Q(s_lobbyGameInfoScrollViewContainerName);

        m_lobbyPageParent = m_root.Q(s_lobbyPageName);
        m_lobbyMenuPanel = m_root.Q(s_lobbyMenuPanelName);
        m_LanguagesPanel = m_root.Q(s_lobbyInformationPanelName);

        m_lobbyGameInfoLeftButton = m_root.Q<Button>(s_lobbyGameInfoLeftButtonName);
        m_lobbyGameInfoRightButton = m_root.Q<Button>(s_lobbyGameInfoRightButtonName);

        m_lobbyTransferTextField = m_root.Q<TextField>(s_lobbyTransferTextFieldName);
        m_lobbyTransferTotalMoneyLabel = m_root.Q<Label>(s_lobbyTransferTotalMoneyLabelName);
        m_lobbyTransferAddButton = m_root.Q<Button>(s_lobbyTransferAddButtonName);

        m_lobbyInformationUserLabel = m_root.Q<Label>(s_lobbyInformationUserLabelName);
        m_lobbyInformationMenuButton = m_root.Q<Button>(s_lobbyInformationMenuButtonName);
        m_lobbyInformationMenuDownButton = m_root.Q<Button>(s_lobbyInformationMenuButtonDownName);

        m_LobbyMenuLanguageButton = m_root.Q<Button>(s_lobbyMenuLanguageButtonName);

        m_LobbyLanguagesCloseButton = m_root.Q<Button>(s_lobbyLanguagesCloseButtonName);
        m_lobbyLanguagesConfirmButton = m_root.Q<Button>(s_lobbyLanguagesConfirmButtonName);

        m_langSelectedElement = m_root.Q(s_languageSelectionElemenName);
    }

    private void RegisterButtonCallbacks()
    {
        // m_lobbyGameInfoLeftButton.RegisterCallback<ClickEvent>(e => LobbyGameInfoPreviousPage());
        // m_lobbyGameInfoRightButton.RegisterCallback<ClickEvent>(e => LobbyGameInfoNextPage());

        m_lobbyTransferAddButton.RegisterCallback<ClickEvent>(OnClickLobbyTransferAddButton);
        m_lobbyInformationMenuButton.RegisterCallback<ClickEvent>(e => MenuPanelEnable(true));
        m_lobbyInformationMenuDownButton.RegisterCallback<ClickEvent>(e => MenuPanelEnable(false));

        m_LobbyMenuLanguageButton.RegisterCallback<ClickEvent>(e => LanguagesPanelEnable(true));

        m_LobbyLanguagesCloseButton.RegisterCallback<ClickEvent>(e => LanguagesPanelEnable(false));
        m_lobbyLanguagesConfirmButton.RegisterCallback<ClickEvent>(e => OnLobbyLanguagesConfirmButton(e));


        // m_lobbyGameInfoScrollViewContainer.RegisterCallback<DragUpdatedEvent>(OnPointerMoveEvent);
    }

    private void SetLanguagesSetting()
    {
        GameManager.Instance.GetGameLanguage().ForEach(language =>
        {
            var element = m_root.Q(s_languageElementName + language);
            element.RegisterCallback<ClickEvent>(e => OnClickLanguage(element, language));
            m_languagesElemant.Add(element);

            if (GameManager.Instance.Language == language)
                OnClickLanguage(element, language);
        });

    }

    // game info
    private IEnumerator GenerateLobbyInfo(UserInfoData userInfoData, GameInfoData gameInfoData)
    {
        int page = 1;
        GameInfoContainer gameInfoContainer = GenerateGameInfoContainerElement(page);


        for (int i = 0; i < gameInfoData.games.Count; i++)
        {
            yield return CreateGameWebUI(gameInfoData.games[i], gameInfoContainer);

            if (gameInfoContainer.GameInfoFull())
            {
                page++;
                gameInfoContainer = GenerateGameInfoContainerElement(page);
            }
        }

        m_lobbyTransferTotalMoneyLabel.text = userInfoData.balance;
        SetInformationUserName();
        GameInfoPageSetting();
        GenerateLobbyPage();

        LoginRespondFinish.Invoke();
    }
    private IEnumerator CreateGameWebUI(GameData gameData, GameInfoContainer gameInfoContainer)
    {
        yield return GenerateGameInfoElement(gameData, gameInfoContainer);
    }
    private GameInfoContainer GenerateGameInfoContainerElement(int page)
    {
        VisualElement element = m_gameInfoContainer.Instantiate();
        // element.AddToClassList(s_gameInfoContainerClass);
        element.AddToClassList(s_gameInfoContainerScrollClass);
        GameInfoContainer gameInfoContainer = new GameInfoContainer();
        gameInfoContainer.SetVisualElements(element, page);
        // m_lobbyGameInfoPanelParent.Add(element);
        m_lobbyGameInfoScrollView.Add(element);

        m_gameInfoContainers.Add(gameInfoContainer);


        return gameInfoContainer;
    }
    private IEnumerator GenerateGameInfoElement(GameData gameData, GameInfoContainer gameInfoContainer)
    {
        VisualElement element = m_gameInfo.Instantiate();
        element.AddToClassList(s_maxSizeClass);

        GameInfo gameInfo = new GameInfo();
        gameInfo.SetVisualElements(element);
        gameInfoContainer.AddGameInfo(gameInfo);

        yield return gameInfo.InitGameWeb(gameData.gameIcon, gameData.gameId, gameData.gameName, () => OnClickWebButton(gameInfo));
    }
    private void OnClickWebButton(GameInfo gameInfo)
    {
        if (m_waitRespond)
            return;

        StartCoroutine(GetGameWebURL(gameInfo.GameId));
    }
    private IEnumerator GetGameWebURL(int gameId)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            GameManager.LoadingStart.Invoke();

        m_waitRespond = true;
        var response = string.Empty;
        Task.Run(async () => response =
            await GameManager.Instance.HttpClientAPI.Oauth(GameManager.s_domain, GameManager.Instance.PhoneNumber, gameId, GameManager.Instance.Language));

        while (response == string.Empty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("GetGameWebURL response = " + response);
        GameWebURLData gameWeb = JsonConvert.DeserializeObject<GameWebURLData>(response);
        m_waitRespond = false;

        OnOpenWebView.Invoke(gameWeb);
        SetLobbyScreenEnable(false);
    }
    private GameInfo GetGameInfo(int gameId)
    {
        for (int i = 0; i < m_gameInfoContainers.Count; i++)
        {
            var gameInfo = m_gameInfoContainers[i].GetGameInfo(gameId);

            if (gameInfo != null)
                return gameInfo;
        }

        return null;
    }


    // page info
    private void GameInfoPageSetting()
    {
        for (int i = 0; i < m_gameInfoContainers.Count; i++)
        {
            if (i == 0)
            {
                m_lobbyCurrentPage = i;
                continue;
            }

            // m_gameInfoContainers[i].Root.AddToClassList(s_gameInfoContainerMoveRightClass);
        }
    }
    private void GenerateLobbyPage()
    {
        for (int i = 0; i < m_gameInfoContainers.Count; i++)
            GenerateLobbyPageElement();

        CurruntPageSetting();
    }
    private VisualElement GenerateLobbyPageElement()
    {

        VisualElement element = new VisualElement();
        element.name = s_lobbyPageName + "-" + m_lobbyPageTotalCount;
        element.AddToClassList(s_lobbyPageBlackClass);

        m_lobbyPageParent.Insert(m_lobbyPageTotalCount + 1, element);
        m_lobbyPageTotalCount++;

        return element;
    }
    private void CurruntPageSetting()
    {
        var page = m_lobbyPageParent.Query(className: s_lobbyPageBlackClass).ToList();
        page.ForEach(element => element.RemoveFromClassList(s_lobbyPageWhiteClass));

        var currentPageElement = page.FirstOrDefault(element => element.name == s_lobbyPageName + "-" + m_lobbyCurrentPage);
        currentPageElement.AddToClassList(s_lobbyPageWhiteClass);
    }
    private void LobbyGameInfoNextPage()
    {
        if (m_gameInfoContainers.Count <= m_lobbyCurrentPage + 1)
            return;

        GameInfoContainer currentContainer = m_gameInfoContainers[m_lobbyCurrentPage];
        GameInfoContainer nextContainer = m_gameInfoContainers[m_lobbyCurrentPage + 1];

        // currentContainer.Root.AddToClassList(s_gameInfoContainerMoveLeftClass);
        // nextContainer.Root.RemoveFromClassList(s_gameInfoContainerMoveRightClass);

        m_lobbyCurrentPage++;
        CurruntPageSetting();
    }
    private void LobbyGameInfoPreviousPage()
    {
        if (m_lobbyCurrentPage <= 0)
            return;


        GameInfoContainer currentContainer = m_gameInfoContainers[m_lobbyCurrentPage];
        GameInfoContainer previousContainer = m_gameInfoContainers[m_lobbyCurrentPage - 1];

        // currentContainer.Root.AddToClassList(s_gameInfoContainerMoveRightClass);
        // previousContainer.Root.RemoveFromClassList(s_gameInfoContainerMoveLeftClass);

        m_lobbyCurrentPage--;
        CurruntPageSetting();
    }

    // information
    private void SetInformationUserName()
    {
        m_lobbyInformationUserLabel.text = GameManager.Instance.UserName;
    }

    // languages
    private void MenuPanelEnable(bool enabled)
    {
        if (enabled)
        {
            Utility.VisualElementDisplayEnable(m_lobbyMenuPanel, true);
            Utility.VisualElementDisplayEnable(m_lobbyInformationMenuButton, false);
            Utility.VisualElementDisplayEnable(m_lobbyInformationMenuDownButton, true);
        }
        else
        {
            Utility.VisualElementDisplayEnable(m_lobbyMenuPanel, false);
            Utility.VisualElementDisplayEnable(m_lobbyInformationMenuButton, true);
            Utility.VisualElementDisplayEnable(m_lobbyInformationMenuDownButton, false);
        }
    }
    private void LanguagesPanelEnable(bool enabled)
    {
        if (enabled)
            m_LanguagesPanel.style.display = DisplayStyle.Flex;
        else
            m_LanguagesPanel.style.display = DisplayStyle.None;
    }

    // network event
    private IEnumerator OnClickTransferAddButton(ClickEvent evt)
    {
        m_onTranferMoney = true;
        TransferBegin.Invoke();

        string response = string.Empty;
        Task.Run(async () => response = await GameManager.Instance.HttpClientAPI.Transfer(GameManager.s_domain, GameManager.Instance.PhoneNumber, m_transferMoney));


        while (response == string.Empty)
            yield return new WaitForSeconds(0.1f);

        Debug.Log("Transfer response : " + response);
        TransferData data = JsonConvert.DeserializeObject<TransferData>(response);

        m_lobbyTransferTotalMoneyLabel.text = data.endMoney;
        m_onTranferMoney = false;
        TransferFinish.Invoke();
    }
    private IEnumerator ChangeLanguage()
    {
        GameManager.LoadingStart.Invoke();

        string response = string.Empty;
        Task.Run(async () => response = await GameManager.Instance.HttpClientAPI.GameInfo(GameManager.s_domain, GameManager.Instance.Language));

        while (response == string.Empty)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GameManager.LoadingProcces.Invoke(50);
        Debug.Log("ChangeLanguage Login GameInfo response = " + response);
        GameInfoData gameInfoData = JsonConvert.DeserializeObject<GameInfoData>(response);

        for (int i = 0; i < gameInfoData.games.Count; i++)
        {
            var gameInfo = GetGameInfo(gameInfoData.games[i].gameId);
            if (gameInfo != null)
            {
                yield return gameInfo.InitGameWeb(gameInfoData.games[i].gameIcon, gameInfoData.games[i].gameId, gameInfoData.games[i].gameName,
                    () => OnClickWebButton(gameInfo));
            }
            else
            {
                Debug.LogError("get game info failed for game id = " + gameInfoData.games[i].gameId);
            }
        }

        GameManager.LoadingProcces.Invoke(100);
    }

    // button event
    private void OnClickLanguage(VisualElement element, string language)
    {
        element.Add(m_langSelectedElement);
        m_choseLanguage = language;
    }
    private void OnLobbyLanguagesConfirmButton(ClickEvent evt)
    {
        LanguagesPanelEnable(false);
        if (GameManager.Instance.Language == m_choseLanguage)
            return;

        GameManager.Instance.Language = m_choseLanguage;
        OnClickLanguageConfirmButton.Invoke();

        if (m_lobbyScreen.style.display == DisplayStyle.Flex)
        {
            StartCoroutine(ChangeLanguage());
        }
    }
    private void OnClickLobbyTransferAddButton(ClickEvent evt)
    {
        if (m_onTranferMoney)
            return;

        StartCoroutine(OnClickTransferAddButton(evt));
    }

    // event
    private void OnCloseWebView()
    {
        SetLobbyScreenEnable(true);

        StartCoroutine(GameManager.Instance.SendAccountInfoRequest());
    }
    private void OnUpdateUserInfoData(UserInfoData data)
    {
        m_lobbyTransferTotalMoneyLabel.text = data.balance;
    }
    private void SetLobbyScreenEnable(bool enabled)
    {
        if (enabled)
            m_lobbyScreen.style.display = DisplayStyle.Flex;
        else
            m_lobbyScreen.style.display = DisplayStyle.None;
    }

}
