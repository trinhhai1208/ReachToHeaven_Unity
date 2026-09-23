using System;
using UnityEngine;

public class CharacterHP : MonoBehaviour, ISubject<Action>
{
    //
    [SerializeField] protected CharacterHealthUI m_healthUI;
    protected CharacterStatManager m_characterStatsManager;

    protected Action m_onGetDamages;
    protected float m_maxHP;
    protected float m_currentHP;
    protected bool m_isDead;

    public event Action OnDeath;

    protected virtual void Awake()
    {
        m_characterStatsManager = GetComponent<CharacterStatManager>();
    }

    protected virtual void Start()
    {
        m_maxHP = m_characterStatsManager.StatDictionary[StatType.Health];
        m_currentHP = m_maxHP;
    }

    ///<summary>
    ///Reset full HP and alive-state when reused from an ObjectPool.
    ///On the very first activation m_maxHP is not set yet, so Start() handles the initial fill.
    /// </summary>
    protected virtual void OnEnable()
    {
        m_isDead = false;
        if (m_maxHP > 0)
        {
            m_currentHP = m_maxHP;
            if (m_healthUI != null) m_healthUI.UpdateHPUI(m_maxHP, m_currentHP);
        }
    }

    public virtual void ResetMaxHealth(float newMaxHP)
    {
        m_maxHP = newMaxHP;
        m_currentHP = newMaxHP;
        m_isDead = false;
        if (m_healthUI != null) m_healthUI.UpdateHPUI(m_maxHP, m_currentHP);
    }

    public virtual void GetDamages(float amount)
    {
        if (m_isDead) return;

        if (amount < m_currentHP) m_currentHP -= amount;
        else
        {
            m_currentHP = 0;
            m_isDead = true;
            OnDeath?.Invoke();
        }

        if (m_healthUI != null) m_healthUI.UpdateHPUI(m_maxHP, m_currentHP);
        m_onGetDamages?.Invoke();
    }

    ///<summary>
    ///Return true if scaleAmount lower or equal currentHP / maxHP
    /// </summary>
    public bool IsHPLowerThanScale(float scaleAmount)
        => (m_currentHP / m_maxHP) <= scaleAmount;

    #region Implement ISubject
    public void Subscribe(Action subscriber)
    {
        m_onGetDamages += subscriber;
    }
    public void UnSubscribe(Action subscriber)
    {
        m_onGetDamages -= subscriber;
    }
    #endregion
}
