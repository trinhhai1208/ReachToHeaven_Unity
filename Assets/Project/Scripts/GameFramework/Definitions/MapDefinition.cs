using System.Collections.Generic;
using UnityEngine;

///<summary>
///Describes a playable map. First version uses a MapRoot prefab (instantiated by
///MapLoader); the data contract stays stable if we later switch to additive scenes.
/// </summary>
[CreateAssetMenu(fileName = "MapDefinition", menuName = "ScriptableObject/Framework/MapDefinition")]
public class MapDefinition : ScriptableObject
{
    //
    [Header("Định Danh (Identity)")]
    public string Id;
    public string DisplayName;
    [TextArea] public string Description;
    public Sprite Preview;

    [Header("Gốc Bản Đồ (prefab cho bản đầu)")]
    [Tooltip("Root containing tilemap/ground, colliders, spawn regions, landmarks, altar")]
    public GameObject MapRootPrefab;

    [Header("Pool Quái Mặc Định (dùng khi wave không có danh sách)")]
    public List<EnemyDefinition> DefaultEnemyPool = new List<EnemyDefinition>();

    [Header("Boss Được Phép Xuất Hiện Trên Map Này")]
    public List<EnemyDefinition> AllowedBosses = new List<EnemyDefinition>();

    [Header("Không Khí / Ánh Sáng (Ambience)")]
    public AudioClip AmbientMusic;
    public Color LightColor = Color.white;
}
