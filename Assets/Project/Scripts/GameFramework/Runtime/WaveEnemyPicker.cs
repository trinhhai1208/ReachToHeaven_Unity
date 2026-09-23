using System.Collections.Generic;
using UnityEngine;

///<summary>
///Pure weighted picker over a wave roster. Deterministic: the caller supplies
///roll01 in [0,1] (e.g. from a seeded RNG), which makes it unit-testable.
/// </summary>
public static class WaveEnemyPicker
{
    //
    public static EnemyDefinition Pick(IReadOnlyList<WaveEnemyEntry> entries, float roll01)
        => PickEntry(entries, roll01)?.Enemy;

    public static WaveEnemyEntry PickEntry(IReadOnlyList<WaveEnemyEntry> entries, float roll01)
    {
        if (entries == null || entries.Count == 0) return null;

        float total = 0f;
        for (int i = 0; i < entries.Count; ++i)
        {
            WaveEnemyEntry entry = entries[i];
            if (entry != null && entry.Enemy != null)
                total += Mathf.Max(0f, entry.Weight);
        }
        if (total <= 0f) return null;

        float target = Mathf.Clamp01(roll01) * total;
        float accumulated = 0f;
        for (int i = 0; i < entries.Count; ++i)
        {
            WaveEnemyEntry entry = entries[i];
            if (entry == null || entry.Enemy == null) continue;

            accumulated += Mathf.Max(0f, entry.Weight);
            if (target <= accumulated) return entry;
        }

        // Fallback for floating-point edge at the upper bound: last valid entry.
        for (int i = entries.Count - 1; i >= 0; --i)
            if (entries[i] != null && entries[i].Enemy != null) return entries[i];

        return null;
    }
}
