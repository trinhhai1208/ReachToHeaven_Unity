using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data-driven configuration for Player base stats.
/// Allows game designers to balance player stats via ScriptableObject assets
/// without modifying the Player prefab or scene files directly.
/// </summary>
[CreateAssetMenu(fileName = "PlayerStatsConfig", menuName = "ScriptableObject/Character/PlayerStatsConfig")]
public class PlayerStatsConfig : ScriptableObject
{
    [Header("Định Danh (Identity)")]
    public string ConfigId = "default_player";
    public string CharacterName = "Player";

    [Header("Chỉ Số Gốc (Base Stats)")]
    [Tooltip("Base values for each StatType")]
    [SerializeField] private List<CharacterStat> m_stats = new List<CharacterStat>()
    {
        new CharacterStat { Type = StatType.Health, Value = 100f },
        new CharacterStat { Type = StatType.Damage, Value = 10f },
        new CharacterStat { Type = StatType.MovementSpeed, Value = 5f },
        new CharacterStat { Type = StatType.AttackCountDown, Value = 1f },
        new CharacterStat { Type = StatType.CritChance, Value = 1f },
        new CharacterStat { Type = StatType.CritDamage, Value = 1.5f },
        new CharacterStat { Type = StatType.Piercing, Value = 1f },
        new CharacterStat { Type = StatType.Regeneration, Value = 1f },
        new CharacterStat { Type = StatType.DamageReduction, Value = 0.1f },
        new CharacterStat { Type = StatType.PickupRange, Value = 0.5f }
    };

    public IReadOnlyList<CharacterStat> Stats => m_stats;

    public List<CharacterStat> CloneStats()
    {
        var list = new List<CharacterStat>(m_stats.Count);
        foreach (var stat in m_stats)
        {
            list.Add(new CharacterStat
            {
                Type = stat.Type,
                Value = stat.Value
            });
        }
        return list;
    }

    public float GetStatValue(StatType type, float fallback = 0f)
    {
        for (int i = 0; i < m_stats.Count; ++i)
        {
            if (m_stats[i].Type == type) return m_stats[i].Value;
        }
        return fallback;
    }
}
