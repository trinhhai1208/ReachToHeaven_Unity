using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : CharacterStatManager, ISubject<Action<StatType, float>>
{
    [Header("Data-Driven Config")]
    [SerializeField] private PlayerStatsConfig m_statsConfig;

    private Dictionary<StatType, float> m_finalStat = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> m_bonusStat = new Dictionary<StatType, float>();
    private Dictionary<StatType, Action> m_updateUIDictionary = new Dictionary<StatType, Action>();
    private Dictionary<StatType, float> m_percentMultiplier = new Dictionary<StatType, float>();

    private Action<StatType, float> m_onChangeValue;

    public override Dictionary<StatType, float> StatDictionary { get => m_finalStat; }
    public PlayerStatsConfig StatsConfig => m_statsConfig;

    public void SetConfig(PlayerStatsConfig config)
    {
        m_statsConfig = config;
        InitStats();
    }

    protected override void Awake()
    {
        InitStats();
    }

    private void InitStats()
    {
        if (m_statsConfig != null)
        {
            m_baseStats.Clear();
            foreach (var stat in m_statsConfig.Stats)
            {
                m_baseStats[stat.Type] = stat.Value;
            }
        }
        else
        {
            base.Awake();
        }

        m_bonusStat.Clear();
        m_percentMultiplier.Clear();
        m_finalStat.Clear();

        foreach (var stat in m_baseStats)
        {
            m_bonusStat[stat.Key] = 0;
            m_percentMultiplier[stat.Key] = 1f;
            m_finalStat[stat.Key] = stat.Value;
        }
    }

    ///<summary>
    ///Registers a stat lazily so upgrading a StatType that isn't in the config
    ///doesn't throw KeyNotFoundException.
    /// </summary>
    private void EnsureStat(StatType type)
    {
        if (!m_baseStats.ContainsKey(type)) m_baseStats[type] = 0f;
        if (!m_bonusStat.ContainsKey(type)) m_bonusStat[type] = 0f;
        if (!m_percentMultiplier.ContainsKey(type)) m_percentMultiplier[type] = 1f;
        if (!m_finalStat.ContainsKey(type)) m_finalStat[type] = m_baseStats[type];
    }

    public void UpgradeStat(CharacterStat statData, AdditionType additionType)
    {
        EnsureStat(statData.Type);

        if (additionType == AdditionType.Flat)
            m_bonusStat[statData.Type] += statData.Value;
        else
            m_percentMultiplier[statData.Type] *= 1f + statData.Value / 100f;

        m_finalStat[statData.Type] =
            (m_baseStats[statData.Type] + m_bonusStat[statData.Type]) * m_percentMultiplier[statData.Type];

        if (m_updateUIDictionary.ContainsKey(statData.Type)) m_updateUIDictionary[statData.Type]?.Invoke();
        if (m_onChangeValue != null) m_onChangeValue.Invoke(statData.Type, m_finalStat[statData.Type]);
    }

    public void Subscribe(StatType type, Action updateEvent)
    {
        if (m_updateUIDictionary.ContainsKey(type)) return;
        m_updateUIDictionary[type] = updateEvent;
    }

    #region Implement ISubject
    public void Subscribe(Action<StatType, float> subscriber)
        => m_onChangeValue += subscriber;
    public void UnSubscribe(Action<StatType, float> subscriber)
        => m_onChangeValue -= subscriber;
    #endregion
}
