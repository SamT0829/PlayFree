using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class GameInfo
{
    private const string s_gameInfoPanelName = "gameinfo-panel";
    private const string s_gameInfoIcon = "gameinfo-icon";
    private const string s_gameInfoName = "gameinfo-name";
    public Action OnClickAction;
    public VisualElement m_root;
    public VisualElement m_gameInfoPanel;
    public VisualElement m_gameInfoIcon;
    public Label m_gameInfoName;


    private int m_gameId;
    public int GameId { get => m_gameId; }


    public void SetVisualElements(VisualElement element)
    {
        m_root = element;
        m_gameInfoPanel = element.Q(s_gameInfoName);
        m_gameInfoIcon = element.Q(s_gameInfoIcon);
        m_gameInfoName = element.Q<Label>(s_gameInfoName);

        element.RegisterCallback<ClickEvent>(OnClickEvent);
    }


    public IEnumerator InitGameWeb(string spriteUrl, int gameId, string gameName, Action onClick)
    {
        m_gameId = gameId;
        m_gameInfoName.text = gameName;
        OnClickAction += onClick;
        yield return GetIconURL(spriteUrl);
    }

    private void OnClickEvent(ClickEvent evt)
    {
        OnClickAction.Invoke();
    }

    private IEnumerator GetIconURL(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            m_gameInfoIcon.style.backgroundImage = texture;
        }
    }
}
