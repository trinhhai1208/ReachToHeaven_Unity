using System.Collections.Generic;
using UnityEngine;

///<summary>
///Data-driven description of an enemy. Wraps the pooled prefab (EnemyProduct)
///plus stats, spawn budget cost and reward. Consumed by WaveDefinition/WaveDirector.
/// </summary>
[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "ScriptableObject/Framework/EnemyDefinition")]
public class EnemyDefinition : ScriptableObject
{
    //
    [Header("Định Danh (Identity)")]
    public string Id;
    public string DisplayName;
    public EnemyRank Rank = EnemyRank.Normal;

    [Header("Spawn (Spawning)")]
    [Tooltip("Prefab root must carry EnemyProduct (IProduct) so the pool can manage it")]
    public EnemyProduct Prefab;
    [Tooltip("Budget cost when a wave picks this enemy; used for wave budgeting")]
    [Min(1)] public int SpawnCost = 1;

    [Header("Chỉ Số Gốc (nguồn chuẩn của quái này)")]
    [Tooltip("Mirrors CharacterStatManager's list so a spawner can apply them")]
    public List<CharacterStat> BaseStats = new List<CharacterStat>();

    [Header("Phần Thưởng (Reward)")]
    [Min(0)] public int ExpReward = 1;

    public float GetBaseStat(StatType type, float fallback = 0f)
    {
        for (int i = 0; i < BaseStats.Count; ++i)
        {
            if (BaseStats[i] != null && BaseStats[i].Type == type)
                return BaseStats[i].Value;
        }
        return fallback;
    }
}
