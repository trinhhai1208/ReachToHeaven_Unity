using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatLog : MonoBehaviour
{
    // 
    [SerializeField] private PlayerStatManager m_playerStatManager;
    [SerializeField] private List<Sprite> m_statSprites = new List<Sprite>();
    [SerializeField] private List<StatLogData> m_statLogDatas = new List<StatLogData>();

    private Dictionary<StatType, StatLogData> m_statDictionary = new Dictionary<StatType, StatLogData>();

    private static readonly StatType[] DisplayStatTypes = new StatType[]
    {
        StatType.AttackCountDown,
        StatType.CritChance,
        StatType.CritDamage,
        StatType.Damage,
        StatType.DamageReduction,
        StatType.Health,
        StatType.MovementSpeed,
        StatType.PickupRange,
        StatType.Piercing,
        StatType.Regeneration
    };

    private void Start()
    {
        for (int i = 0; i < m_statLogDatas.Count && i < DisplayStatTypes.Length; ++i)
        {
            StatType type = DisplayStatTypes[i];
            if (m_playerStatManager.StatDictionary.TryGetValue(type, out float value))
            {
                m_statDictionary[type] = m_statLogDatas[i];
                Sprite sprite = i < m_statSprites.Count ? m_statSprites[i] : null;
                m_statLogDatas[i].InitData(sprite, value);
            }
        }
    }

    private void OnEnable()
        => m_playerStatManager.Subscribe(UpdateStat);

    private void OnDisable()
        => m_playerStatManager.UnSubscribe(UpdateStat);

    private void UpdateStat(StatType type, float value)
    {
        if (m_statDictionary.TryGetValue(type, out StatLogData data))
            data.UpdateValue(value);
    }

}
