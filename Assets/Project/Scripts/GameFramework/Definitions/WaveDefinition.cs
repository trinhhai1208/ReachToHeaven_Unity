using System.Collections.Generic;
using UnityEngine;

///<summary>
///A single wave: when it starts, who spawns, how many portals and the
///difficulty scaling applied to those enemies. Pure data; WaveDirector drives it.
/// </summary>
[CreateAssetMenu(fileName = "WaveDefinition", menuName = "ScriptableObject/Framework/WaveDefinition")]
public class WaveDefinition : ScriptableObject
{
    //
    [Header("Thời Điểm (giây, tính từ lúc bắt đầu màn)")]
    [Min(0f)] public float StartTime;
    [Min(0f)] public float Duration = 30f;

    [Header("Danh Sách Quái (theo trọng số)")]
    public List<WaveEnemyEntry> Enemies = new List<WaveEnemyEntry>();

    [Header("Spawn Cổng (Portal)")]
    [Min(1)] public int PortalCount = 3;
    [Min(1)] public int EnemiesPerPortal = 5;
    [Min(0.1f)] public float SpawnInterval = 1f;

    [Header("Hệ Số Độ Khó (nhân lên chỉ số gốc của quái)")]
    [Min(0.01f)] public float HealthScale = 1f;
    [Min(0.01f)] public float DamageScale = 1f;
    [Min(0.01f)] public float SpeedScale = 1f;

    [Header("Luật (Rules)")]
    [Range(0f, 1f)] public float EliteChance;
    [Tooltip("Upper bound of simultaneously alive enemies this wave pushes toward")]
    [Min(1)] public int AliveCap = 60;
}
