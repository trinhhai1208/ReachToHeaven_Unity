using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneManager : Singleton<BaseSceneManager>
{
    //
    [SerializeField] protected List<GraphicList> m_graphicLists = new List<GraphicList>();

    protected Dictionary<GraphicTag, List<Action<bool>>> m_graphicDictionary 
        = new Dictionary<GraphicTag, List<Action<bool>>>();

    protected virtual void Start()
    {
        MapDictionary();
        SetGraphics();

        SingletonUIManager.Instance.SettingPanel.GraphicManager.UpdateGraphic(m_graphicDictionary);
    }

    protected void MapDictionary()
    {
        foreach (var graphicList in m_graphicLists)
        {
            List<Action<bool>> actionList = new List<Action<bool>>();

            foreach (var leader in graphicList.LeadersWithTag)
                actionList.Add((bool activeState) => leader.SetActive(activeState));

            m_graphicDictionary[graphicList.GraphicTag] = actionList;
        }
    }

    protected void SetGraphics()
    {
        Dictionary<GraphicTag, bool> activeDictionary
            = SingletonUIManager.Instance.SettingPanel.GraphicManager.ActiveStateDictionary;

        foreach(var graphicUnit in m_graphicDictionary)
        {
            if (!activeDictionary.ContainsKey(graphicUnit.Key)) continue;

            GraphicTag tag = graphicUnit.Key;
            bool activeState = activeDictionary[tag];

            foreach(var graphicElement in graphicUnit.Value)
            {
                graphicElement.Invoke(activeState);
            }
        }
    }

    protected void SetupGraphicList()
    {
        SingletonUIManager.Instance.SettingPanel.GraphicManager.UpdateGraphic(m_graphicDictionary);
    }
}
