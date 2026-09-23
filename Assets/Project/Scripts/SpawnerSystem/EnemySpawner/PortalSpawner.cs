using System;
using UnityEngine;
using System.Collections;

public class PortalSpawner : Spawner
{
    //
    [SerializeField] private Rigidbody2D m_playerRigidbody;
    [SerializeField] private Transform m_enemyField;
    [SerializeField] private int m_totalPortal;
    [SerializeField] private float m_minIntervalScale = 0.4f;

    [Tooltip("When true, PortalSpawner stops self-driving and waits for WaveDirector.ExecuteWave()")]
    [SerializeField] private bool m_drivenByWaveDirector;

    #region Supporter
    [SerializeField] private IntervalRandomizer m_intervalRandomizer = new IntervalRandomizer();
    [SerializeField] private PositionRandomizer m_positionRandomizer = new PositionRandomizer();
    #endregion

    private int m_portalCount;
    private WaveDefinition m_activeWave;

    public int PortalCount => m_portalCount;
    public int TotalPortal => m_totalPortal;
    public Transform EnemyField => m_enemyField;

    public event Action<int> OnPortalSpawned;

    private void Start()
    {
        m_portalCount = 0;
        m_positionRandomizer.Init(m_playerRigidbody.transform);
        m_onExtraSetup = SetupPortalProduct;
        SetupProductPools();

        // Legacy self-driving path stays the default so existing scenes are unchanged.
        // When driven by WaveDirector, portal timing/count comes from WaveDefinition instead.
        if (!m_drivenByWaveDirector)
            StartCoroutine(SpawnRoutine(5));
    }

    ///<summary>
    ///Executor entry point for WaveDirector.OnWaveStarted. Spawns this wave's portals
    ///at randomized positions on the wave's interval. Progression (which wave, when)
    ///lives in WaveDirector; this method only performs the spawn.
    /// </summary>
    public void ExecuteWave(WaveDefinition wave)
    {
        if (wave == null) return;
        m_activeWave = wave;
        StartCoroutine(ExecuteWaveRoutine(wave));
    }

    ///<summary>Stamp the active wave onto each portal BEFORE it activates, so the
    ///portal's EnemySpawner (OnEnable) can read wave count/interval.</summary>
    protected override void OnGetProduct(IProduct product)
    {
        ProductConverter.IProductToAnyType<PortalProduct>(product).SetActiveWave(m_activeWave);
        base.OnGetProduct(product);
    }

    private IEnumerator ExecuteWaveRoutine(WaveDefinition wave)
    {
        int spawnedThisWave = 0;
        int portalCount = wave.PortalCount;
        LevelDefinition level = RunConfig.Current != null ? RunConfig.Current.Level : null;
        if (level != null && level.PortalCountOverride > 0) portalCount = level.PortalCountOverride;
        while (spawnedThisWave < portalCount)
        {
            PortalProduct product = Spawn<PortalProduct>(0);
            product.gameObject.transform.position = m_positionRandomizer.RandomizePosition();
            ++m_portalCount;
            ++spawnedThisWave;
            OnPortalSpawned?.Invoke(m_portalCount);
            yield return new WaitForSeconds(wave.SpawnInterval);
        }
    }

    private void SetupPortalProduct(IProduct product)
    {
        PortalProduct portalProduct = ProductConverter.IProductToAnyType<PortalProduct>(product);
        portalProduct.Init(m_playerRigidbody, m_enemyField);
    }

    private float GetIntervalScale()
    {
        float progress = m_totalPortal > 0 ? (float)m_portalCount / m_totalPortal : 0f;
        return Mathf.Lerp(1f, m_minIntervalScale, progress);
    }

    private IEnumerator SpawnRoutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        while (m_portalCount < m_totalPortal)
        {
            PortalProduct product = Spawn<PortalProduct>(0);
            product.gameObject.transform.position = m_positionRandomizer.RandomizePosition();
            ++m_portalCount;
            OnPortalSpawned?.Invoke(m_portalCount);
            yield return new WaitForSeconds(m_intervalRandomizer.RandomizeInterval() * GetIntervalScale());
        }
    }
}
