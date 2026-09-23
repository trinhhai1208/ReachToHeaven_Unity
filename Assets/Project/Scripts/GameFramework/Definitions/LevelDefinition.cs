using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data-driven definition of a playable Level.
/// Serializable inline class — authored entirely inside the single shared <see cref="LevelCatalog"/> asset,
/// so designers manage every level in one place instead of separate ScriptableObject files.
/// </summary>
[Serializable]
public class LevelDefinition
{
    [Header("Thông Tin Màn Chơi")]
    [Tooltip("Số thứ tự của màn chơi (1, 2, 3, ...)")]
    [Min(1)] public int LevelNumber = 1;

    [Tooltip("Tên hiển thị trên giao diện chọn màn")]
    public string DisplayName = "Level 1";

    [Tooltip("Mô tả ngắn hiển thị khi người chơi chọn màn này")]
    [TextArea] public string Description = "Survive the oncoming waves of darkness.";

    [Tooltip("Ảnh đại diện của màn chơi (tùy chọn)")]
    public Sprite Thumbnail;

    [Header("Thời Gian & Nhịp Độ")]
    [Tooltip("Tổng thời gian sinh tồn tính bằng giây (vd: 60 = 1 phút, 90 = 1 phút 30, 120 = 2 phút).")]
    [Min(10f)] public float Duration = 60f;

    [Tooltip("Khoảng cách cố định giữa các wave (giây) để đồng bộ UI và nhịp spawn quái.")]
    [Min(5f)] public float WaveInterval = 20f;

    [Header("Số Lượng Spawn (0 = dùng theo Wave)")]
    [Tooltip("Số CỔNG spawn mỗi wave cho màn này. 0 = dùng giá trị trong WaveDefinition.")]
    [Min(0)] public int PortalCountOverride = 0;

    [Tooltip("Số QUÁI mỗi cổng cho màn này. 0 = dùng giá trị trong WaveDefinition. Giảm số này để bớt quái.")]
    [Min(0)] public int EnemiesPerPortalOverride = 0;

    [Header("Bản Đồ")]
    [Tooltip("Bản đồ được sử dụng cho màn chơi này")]
    public MapDefinition Map;

    [Header("Danh Sách Wave")]
    [Tooltip("Các wave được thiết kế sẵn cho màn chơi. Mỗi wave định nghĩa quái, số cổng và hệ số chỉ số.")]
    public List<WaveDefinition> Waves = new List<WaveDefinition>();

    /// <summary>
    /// Computes total number of waves either from the authored list or from Duration / WaveInterval.
    /// </summary>
    public int TotalWaves
    {
        get
        {
            if (Waves != null && Waves.Count > 0) return Waves.Count;
            return Mathf.Max(1, Mathf.FloorToInt(Duration / Mathf.Max(1f, WaveInterval)));
        }
    }

    /// <summary>
    /// Gets wave definition by index safely (with fallback if list is smaller).
    /// </summary>
    public WaveDefinition GetWave(int index)
    {
        if (Waves == null || Waves.Count == 0) return null;
        return Waves[Mathf.Clamp(index, 0, Waves.Count - 1)];
    }
}
