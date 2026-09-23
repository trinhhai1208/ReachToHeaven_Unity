using System;
using UnityEngine;

///<summary>
///One weighted row of a wave roster. Weight is relative within the wave.
/// </summary>
[Serializable]
public class WaveEnemyEntry
{
    public EnemyDefinition Enemy;
    [Min(0f)] public float Weight = 1f;

    [Header("Ghi Đè Chỉ Số Theo Wave (0 = dùng chỉ số gốc)")]
    [Tooltip("If > 0, overrides enemy base health for this wave")]
    public float CustomHealth = 0f;

    [Tooltip("If > 0, overrides enemy base damage for this wave")]
    public float CustomDamage = 0f;

    [Tooltip("If > 0, overrides enemy base movement speed for this wave")]
    public float CustomSpeed = 0f;
}
