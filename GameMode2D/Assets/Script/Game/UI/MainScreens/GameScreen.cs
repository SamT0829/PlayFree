using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameScreen : MonoBehaviour
{
    public static Action OnCloseWebView;

    [SerializeField] protected UIDocument m_Document;


    private const string s_gameScreenName = "game-screen";
    private const string s_gameScreenHomeButtonName = "game-screen_home-button";

    private const string s_gameScreenHomeButtonWClass = "home-button-w";
    private const string s_gameScreenHomeButtonVClass = "home-button-v";


    private VisualElement m_root;
    private VisualElement m_gameScreen;

    private Button m_homeButton;

    private bool m_uniWebViewClose = true;

    private void Awake()
    {
        m_root = m_Document.rootVisualElement;

        SetVisualElements();
        RegisterButtonCallbacks();
    }

    private void OnEnable()
    {
        LobbyScreen.OnOpenWebView += OnOpenWebView;
        GameManager.OnSwithUI += OnSwithUI;

        UniWebViewController.OnUniWebViewPageClose += () => m_uniWebViewClose = true;
    }

    private void OnDisable()
    {
        LobbyScreen.OnOpenWebView -= OnOpenWebView;
        GameManager.OnSwithUI -= OnSwithUI;

        UniWebViewController.OnUniWebViewPageClose -= () => m_uniWebViewClose = true;
    }

    private void SetVisualElements()
    {
        m_homeButton = m_root.Q<Button>(s_gameScreenHomeButtonName);
        m_gameScreen = m_root.Q(s_gameScreenName);
    }
    private void RegisterButtonCallbacks()
    {
        m_homeButton.RegisterCallback<ClickEvent>(e =>
        {
            Screen.orientation = ScreenOrientation.LandscapeRight;
            Utility.SetScreenOrientation(true, true, false, false);
            StartCoroutine(OnClickHomeButton(e));
        });
    }

    private void SetGameScreenEnable(bool enabled)
    {
        if (enabled)
            m_gameScreen.style.display = DisplayStyle.Flex;
        else
            m_gameScreen.style.display = DisplayStyle.None;

        OnSwithUI();
    }

    // event

    private IEnumerator OnClickHomeButton(ClickEvent evt)
    {
        GameManager.LoadingStart.Invoke();
        GameManager.Instance.UniWebViewController.CloseWeb();

        if (!m_uniWebViewClose)
            yield return new WaitForSeconds(0.1f);

        SetGameScreenEnable(false);
        OnCloseWebView.Invoke();
    }

    private void OnOpenWebView(GameWebURLData gameWebData)
    {
        SetGameScreenEnable(enabled);
        GameManager.Instance.UniWebViewController.OpenWeb(gameWebData.interface2);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameManager.LoadingProcces(50);
            m_uniWebViewClose = false;
        }
    }

    private void OnSwithUI()
    {
        if (GameManager.Instance.switchUI == "W")
        {
            m_homeButton.RemoveFromClassList(s_gameScreenHomeButtonVClass);
            m_homeButton.AddToClassList(s_gameScreenHomeButtonWClass);
        }
        if (GameManager.Instance.switchUI == "V")
        {
            m_homeButton.RemoveFromClassList(s_gameScreenHomeButtonWClass);
            m_homeButton.AddToClassList(s_gameScreenHomeButtonVClass);
        }
    }
}
