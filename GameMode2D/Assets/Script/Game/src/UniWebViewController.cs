using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniWebViewController : MonoBehaviour
{
    public static Action OnUniWebViewPageFinish;
    public static Action OnUniWebViewPageClose;

    private UniWebView m_uniWebView;

    private void OnEnable()
    {
        GameManager.OnSwithUI += SwitchWebView;
    }

    private void OnDisable()
    {
        GameManager.OnSwithUI += SwitchWebView;
    }

    public void OpenWeb(string m_url)
    {
        if (m_uniWebView == null)
            UniWebViewMethod(transform);

        else
            return;

        m_uniWebView.CleanCache();
        m_uniWebView.Load(m_url);
    }

    public void CloseWeb()
    {
        m_uniWebView.CleanCache();
        m_uniWebView.Hide();

        Destroy(m_uniWebView.gameObject);
        m_uniWebView = null;
    }

    private bool OnShouldClose(UniWebView webView)
    {
        m_uniWebView = null;
        OnUniWebViewPageClose.Invoke();
       
        return true;
    }

    private void OnLoadingErrorReceived(UniWebView webView, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
    {
        // m_state.text = "Loading Error !!! " + errorCode;
    }

    private void OnPageStart(UniWebView webView, string url)
    {
        m_uniWebView.Hide();
        // m_state.text = "Loading......!!!";
    }

    public void ShowUniWeb()
    {
        m_uniWebView.Show();
    }

    private void OnPageFinish(UniWebView webView, int statusCode, string url)
    {
        // m_state.text = "Finish!!!";
        OnUniWebViewPageFinish.Invoke();
    }

    private void UniWebViewMethod(Transform parent)
    {
        GameObject webView = new GameObject();
        m_uniWebView = webView.AddComponent<UniWebView>();
        // m_uniWebView.Frame = new Rect(0, Screen.height * 0.1f, Screen.width, Screen.height - (Screen.height * 0.1f));
        m_uniWebView.transform.parent = parent;
        // m_uniWebView.SetBackButtonEnabled(true);
        // m_uniWebView.SetZoomEnabled(true);
        m_uniWebView.SetLoadWithOverviewMode(true);
        // m_uniWebView.SetHorizontalScrollBarEnabled(true);
        // m_uniWebView.SetVerticalScrollBarEnabled(true);

        m_uniWebView.OnPageStarted += OnPageStart;
        m_uniWebView.OnPageFinished += OnPageFinish;
        m_uniWebView.OnLoadingErrorReceived += OnLoadingErrorReceived;
        m_uniWebView.OnShouldClose += OnShouldClose;

        // m_uniWebView.OnOrientationChanged += (view, orientation) =>
        // {
        //     // Set full screen again. If it is now in landscape, it is 640x320.
        //     m_uniWebView.Frame = new Rect(0, Screen.height * 0.1f, Screen.width, Screen.height - (Screen.height * 0.1f));
        //     m_uniWebView.UpdateFrame();
        // };

        SwitchWebView();
    }

    private void SwitchWebView()
    {
        if (m_uniWebView == null)
            return;

        if (GameManager.Instance.switchUI == "W")
        {
            var heightHeader = GameManager.Instance.hight * 0.1f;
            m_uniWebView.Frame = new Rect(0, heightHeader, GameManager.Instance.width, GameManager.Instance.hight - heightHeader);

        }
        if (GameManager.Instance.switchUI == "V")
        {
            var heightHeader = GameManager.Instance.width * 0.1f;
            m_uniWebView.Frame = new Rect(0, heightHeader, GameManager.Instance.hight, GameManager.Instance.width - heightHeader);
        }
    }
}
