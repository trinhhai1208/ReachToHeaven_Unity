using System.Collections.Generic;
using UnityEngine;

///<summary>
///Describes a game mode: end condition, its ordered wave list and the
///per-cycle scaling used by endless-style modes. Reused across maps.
/// </summary>
[CreateAssetMenu(fileName = "GameModeDefinition", menuName = "ScriptableObject/Framework/GameModeDefinition")]
public class GameModeDefinition : ScriptableObject
{
    //
    [Header("Định Danh (Identity)")]
    public string Id;
    public string DisplayName;
    public GameModeType ModeType = GameModeType.Survival;

    [Header("Điều Kiện Kết Thúc")]
    [Tooltip("Run length in seconds. 0 means no time-based victory (endless)")]
    [Min(0f)] public float Duration = 300f;

    [Header("Danh Sách Wave (thứ tự bất kỳ; tự sắp theo StartTime)")]
    public List<WaveDefinition> Waves = new List<WaveDefinition>();

    [Header("Hệ Số Vô Tận (mỗi vòng; chế độ có thời hạn bỏ qua)")]
    [Min(1f)] public float CycleHealthScale = 1.15f;
    [Min(1f)] public float CycleDamageScale = 1.1f;
}
