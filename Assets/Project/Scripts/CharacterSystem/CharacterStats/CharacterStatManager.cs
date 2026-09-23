using System.Collections.Generic;
using UnityEngine;

public class CharacterStatManager : MonoBehaviour
{
    //
    [SerializeField] protected List<CharacterStat> m_characterStat = new List<CharacterStat>();
    
    protected Dictionary<StatType, float> m_baseStats = new Dictionary<StatType, float>();

    public virtual Dictionary<StatType, float> StatDictionary { get => m_baseStats; }

    public bool CheckStateDict;

    ///<summary>
    ///Mapping stat's dictionary
    /// </summary>
    protected virtual void Awake()
    {
        if (CheckStateDict) Debug.Log(this + "=================================");
        foreach(CharacterStat characterStat in m_characterStat)
        {
            if (m_baseStats.TryGetValue(characterStat.Type, out float value))
            {
                Debug.LogWarning($"{this}: {characterStat.Type}'s value is overrided: {value} -> {characterStat.Value}");
            }                

            m_baseStats[characterStat.Type] = characterStat.Value;

            if (CheckStateDict) Debug.Log(characterStat.Type + "->" + characterStat.Value);
        }
        
        if (CheckStateDict) Debug.Log("======================================");
    }

    public bool HasStat(StatType type)
        => m_baseStats.ContainsKey(type);

    public void SetStat(StatType type, float value)
    {
        m_baseStats[type] = value;
    }
}