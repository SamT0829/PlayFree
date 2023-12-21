using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class GameInfoContainer
{
    private const string s_gameInfoContainerName = "gameinfo-container";
    private const string s_gameInfoPointName = "gameInfo-point-";

    private List<VisualElement> m_gameInfoParentList = new();
    private List<GameInfo> m_gameInfoList = new();

    public int Page;
    public VisualElement Root;
    public VisualElement gameInfoContainer;

    public void SetVisualElements(VisualElement element, int page)
    {
        Root = element;
        gameInfoContainer = element.Q(s_gameInfoContainerName);

        for (int i = 1; i <= 6; i++)
        {
            var gameInfoPoint = element.Q(s_gameInfoPointName + i);
            m_gameInfoParentList.Add(gameInfoPoint);
        }

        Page = page;
    }

    public void AddGameInfo(GameInfo gameInfo)
    {
        m_gameInfoList.Add(gameInfo);

        var index = m_gameInfoList.IndexOf(gameInfo);
        m_gameInfoParentList[index].Add(gameInfo.m_root);
    }

    public bool GameInfoFull()
    {
        return m_gameInfoParentList.Count == m_gameInfoList.Count;
    }

    public GameInfo GetGameInfo(int gameId)
    {
        return m_gameInfoList.FirstOrDefault(gameInfo => gameInfo.GameId == gameId);
    }
}
