using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private UIDocument m_Document;

    private const string s_loadingScreenName = "loading-screen";
    private const string s_loadingBarPanelName = "loading-loadingbar-screen";
    private const string s_loadingProgressbarName = "loading-progressbar";

    private const string s_loadingCircleName = "loading-circle-panel";
    private const string s_loadingCircleIconName = "loading-circle-icon";

    private const string s_loadingIconSpinClass = "loading-icon-spin";



    private VisualElement m_root;
    private VisualElement m_loadingScreen;
    private VisualElement m_loadingBarPanel;
    private ProgressBar m_loadingProgressBar;

    private VisualElement m_loadingCirclePanel;
    private VisualElement m_loadingCircleIcon;


    private void Awake()
    {
        m_root = m_Document.rootVisualElement;

        SetVisualElements();
    }

    private void OnEnable()
    {
        GameManager.LoadingStart += OnShowLoadingBarPanel;
        GameManager.LoadingProcces += OnLoadingProcess;

        LobbyScreen.TransferBegin += OnShowLoadingCirclePanel;
        LobbyScreen.TransferFinish += OnHideLoadingCirclePanel;

        UniWebViewController.OnUniWebViewPageFinish += () => StartCoroutine(OnUniWebViewPageFinish());
    }



    private void OnDisable()
    {
        GameManager.LoadingStart -= OnShowLoadingBarPanel;
        GameManager.LoadingProcces -= OnLoadingProcess;

        LobbyScreen.TransferBegin -= OnShowLoadingCirclePanel;
        LobbyScreen.TransferFinish -= OnHideLoadingCirclePanel;

        UniWebViewController.OnUniWebViewPageFinish -= () => StartCoroutine(OnUniWebViewPageFinish());
    }


    private void SetVisualElements()
    {
        m_loadingScreen = m_root.Q(s_loadingScreenName);
        m_loadingBarPanel = m_root.Q(s_loadingBarPanelName);
        m_loadingProgressBar = m_root.Q<ProgressBar>(s_loadingProgressbarName);

        m_loadingCirclePanel = m_root.Q(s_loadingCircleName);
        m_loadingCircleIcon = m_root.Q(s_loadingCircleIconName);
    }

    private void OnShowLoadingBarPanel()
    {
        Utility.VisualElementDisplayEnable(m_loadingScreen, true);
        Utility.VisualElementDisplayEnable(m_loadingBarPanel, true);
        Utility.VisualElementDisplayEnable(m_loadingCirclePanel, false);
        m_loadingProgressBar.value = m_loadingProgressBar.lowValue;
    }

    private void OnLoadingProcess(int value)
    {
        StartCoroutine(LoadingProcess(value));
    }

    private IEnumerator LoadingProcess(int value)
    {
        while (m_loadingProgressBar.value < value)
        {
            yield return new WaitForSeconds(0.001f);
            m_loadingProgressBar.value++;
        }

        if (m_loadingProgressBar.value == 100)
            Utility.VisualElementDisplayEnable(m_loadingScreen, false);
    }

    private IEnumerator OnUniWebViewPageFinish()
    {
        yield return LoadingProcess(100);
        GameManager.Instance.UniWebViewController.ShowUniWeb();

        Utility.SetScreenOrientation(true, true, true, true);
    }

    private void OnShowLoadingCirclePanel()
    {
        Utility.VisualElementDisplayEnable(m_loadingScreen, true);
        Utility.VisualElementDisplayEnable(m_loadingBarPanel, false);
        Utility.VisualElementDisplayEnable(m_loadingCirclePanel, true);

        m_loadingCircleIcon.AddToClassList(s_loadingIconSpinClass);
    }

    private void OnHideLoadingCirclePanel()
    {
        Utility.VisualElementDisplayEnable(m_loadingScreen, false);
        Utility.VisualElementDisplayEnable(m_loadingBarPanel, false);
        Utility.VisualElementDisplayEnable(m_loadingCirclePanel, false);

        m_loadingCircleIcon.RemoveFromClassList(s_loadingIconSpinClass);
    }
}
