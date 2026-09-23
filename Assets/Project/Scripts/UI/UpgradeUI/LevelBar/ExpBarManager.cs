using UnityEngine;

public class ExpBarManager : MonoBehaviour
{
    //
    [SerializeField] private UpgradeStatManager m_upgradeManager;
    [SerializeField] private ExpBarUI m_expBarUI = new ExpBarUI();

    private int m_currentLevel = 1;
    private float m_maxExp;
    private float m_currentExp;

    private void Start()
    {
        m_maxExp = GetNextMaxExp(m_currentLevel);
        m_currentExp = 0;

        m_expBarUI.UpdateUI(0);
    }

    public void UpdateExpAmount(float amount)
    {
        m_currentExp += amount;

        while (m_currentExp >= m_maxExp)
        {
            m_currentExp -= m_maxExp;
            ++m_currentLevel;
            m_maxExp = GetNextMaxExp(m_currentLevel);
            m_upgradeManager.QueueLevelUp();
        }

        m_expBarUI.UpdateUI(m_currentExp / m_maxExp, m_currentLevel);
    }

    ///<summary>
    ///Calculate next exp to level up based on current level
    /// </summary>
    private float GetNextMaxExp(float currentLevel)
        => 2 * Mathf.Pow(currentLevel, 1.5f);
}
