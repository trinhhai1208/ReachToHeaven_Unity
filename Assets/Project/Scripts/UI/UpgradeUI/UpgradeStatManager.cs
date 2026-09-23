using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeStatManager : MonoBehaviour
{
    //
    [SerializeField] private PlayerStatManager m_statManager;
    [SerializeField] private InputController m_inputController;
    [SerializeField] private AudioClip m_levelUpSFX;
    [SerializeField] private AudioClip m_flipCardSFX;
    [SerializeField] private AudioClip m_statUpSFX;
    
    
    [SerializeField] private List<UpgradeStatPanel> m_upgradeStatPanels = new List<UpgradeStatPanel>();
    [SerializeField] private List<UpgradeStat> m_upgradeStats = new List<UpgradeStat>();


    private readonly List<int> m_index = new List<int>();
    private readonly List<int> m_candidates = new List<int>();

    private int m_pendingLevelUps;

    ///<summary>
    ///Request one upgrade selection. Multiple calls (e.g. gaining several levels from one gem)
    ///are queued and presented one after another.
    /// </summary>
    public void QueueLevelUp()
    {
        ++m_pendingLevelUps;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        GameManager.FreezeScreen();
        EventAudioManager.Instance.PlayEventSFX(m_levelUpSFX);
        m_inputController.DisableInput();

        ShowCards();
    }

    private void ShowCards()
    {
        SetupIndexes();

        if (m_index.Count == 0)
        {
            Debug.LogWarning($"{this}: No upgrade stats available; skipping level-up to avoid soft-lock.");
            m_pendingLevelUps = 0;
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < m_upgradeStatPanels.Count; ++i)
        {
            if (i < m_index.Count)
            {
                m_upgradeStatPanels[i].Init(m_upgradeStats[m_index[i]], m_flipCardSFX);
                m_upgradeStatPanels[i].gameObject.SetActive(true);
            }
            else m_upgradeStatPanels[i].gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        GameManager.UnFreezeScreen();
        m_inputController.EnableInput();
    }

    ///<summary>
    ///Picks distinct upgrade indices, capped by however many stats and panels exist.
    ///Safe when the pool has fewer than 3 entries (partial Fisher-Yates shuffle).
    /// </summary>
    private void SetupIndexes()
    {
        m_index.Clear();

        int statCount = m_upgradeStats.Count;
        int cardCount = Mathf.Min(m_upgradeStatPanels.Count, statCount);
        if (cardCount <= 0) return;

        m_candidates.Clear();
        for (int i = 0; i < statCount; ++i) m_candidates.Add(i);

        for (int i = 0; i < cardCount; ++i)
        {
            int swap = Random.Range(i, statCount);
            (m_candidates[i], m_candidates[swap]) = (m_candidates[swap], m_candidates[i]);
            m_index.Add(m_candidates[i]);
        }
    }

    public void OnChooseStat(int index)
    {
        EventAudioManager.Instance.PlayEventSFX(m_statUpSFX);

        UpgradeStat upgradeStat = m_upgradeStats[m_index[index]];
        m_statManager.UpgradeStat(upgradeStat.UpgradeStatSystemData, upgradeStat.AdditionType);

        --m_pendingLevelUps;
        if (m_pendingLevelUps > 0) ShowCards();
        else gameObject.SetActive(false);
    }
}
