using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single shared ScriptableObject that holds EVERY level of the game inline.
/// One asset = the full level list, edited in one Inspector.
/// Consumed by LevelSelectUI to populate the level grid and pagination.
/// </summary>
[CreateAssetMenu(fileName = "LevelCatalog", menuName = "ScriptableObject/Framework/LevelCatalog")]
public class LevelCatalog : ScriptableObject
{
    [Header("Danh Sách Toàn Bộ Màn Chơi")]
    [Tooltip("Toàn bộ các màn chơi, sắp theo thứ tự. Chỉnh sửa mọi level ngay tại đây.")]
    public List<LevelDefinition> Levels = new List<LevelDefinition>();

    public int Count => Levels != null ? Levels.Count : 0;

    public LevelDefinition GetLevel(int index)
    {
        if (Levels == null || index < 0 || index >= Levels.Count) return null;
        return Levels[index];
    }
}
