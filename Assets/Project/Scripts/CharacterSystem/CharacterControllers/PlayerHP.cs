using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
public class PlayerHP : CharacterHP
{
    //
    [FormerlySerializedAs("m_regenateInterval")]
    [SerializeField] private float m_regenerateInterval;

    protected override void Start()
    {
        base.Start();
        if (m_characterStatsManager is PlayerStatManager playerStatManager)
            playerStatManager.Subscribe(StatType.Health, UpdateMaxHealth);

        StartCoroutine(Regenerate());
    }

    public override void GetDamages(float amount)
    {
        if (m_isDead) return;

        if (m_characterStatsManager.StatDictionary.TryGetValue(StatType.DamageReduction, out float reduction))
            amount = ReduceDamage(amount, reduction);

        base.GetDamages(amount);
        GameEvents.TriggerPlayerHPChanged(m_currentHP, m_maxHP);

        if (m_isDead)
        {
            GameEvents.TriggerPlayerDied();
        }
    }

    private float ReduceDamage(float amount, float reduceAmount)
    {
        reduceAmount = Mathf.Clamp01(reduceAmount);
        amount -= (reduceAmount * amount);
        if (amount < 0) amount = 0;

        return amount;
    }

    private void UpdateMaxHealth()
    {
        m_maxHP = m_characterStatsManager.StatDictionary[StatType.Health];
        m_healthUI.UpdateHPUI(m_maxHP, m_currentHP);
    }

    private IEnumerator Regenerate()
    {
        while (true)
        {
            if (!m_isDead && m_currentHP < m_maxHP)
            {
                m_currentHP =
                    Mathf.Clamp(m_currentHP += m_characterStatsManager.StatDictionary[StatType.Regeneration], 0, m_maxHP);
                m_healthUI.UpdateHPUI(m_maxHP, m_currentHP);
                //Debug.Log("Regen");
                yield return new WaitForSeconds(m_regenerateInterval);
            }

            yield return null;
        }
    }
}
