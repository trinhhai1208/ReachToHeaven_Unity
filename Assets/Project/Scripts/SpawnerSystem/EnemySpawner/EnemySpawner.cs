using System.Collections;
using UnityEngine;

public class EnemySpawner : Spawner
{
    //

    private PortalProduct m_portalProduct;
    private Animator m_animator;
    private int m_spawnCount;

    #region Supporter
    [SerializeField] private CountRandomizer m_countRandomizer = new CountRandomizer(); 
    [SerializeField] private IntervalRandomizer m_intervalRandomizer = new IntervalRandomizer();
    private IndexRandomizer m_indexRandonmizer = new IndexRandomizer();

    [SerializeField] private AudioPlayerSupporter m_audioPlayer = new AudioPlayerSupporter();
    #endregion

    private void Awake()
    {
        m_portalProduct = GetComponent<PortalProduct>();
        m_animator = GetComponent<Animator>();

        m_audioPlayer.Init(GetComponent<AudioSource>());
    }

    private void Start()
    {
        m_productField = m_portalProduct.EnemyField;

        m_onExtraSetup = SetupEnemyProduct;
        SetupProductPools();

        m_indexRandonmizer.Init(m_productPrefabs.Count);
    }

    private void OnEnable()
    {
        m_spawnCount = 0;
        m_audioPlayer.PlayOneShot(0);
        StartCoroutine(SpawnEnemyRoutine());
    }

    private void SetupEnemyProduct(IProduct product)
        => ProductConverter.IProductToAnyType<EnemyProduct>(product).Init(m_portalProduct.PlayerRigidbody);

    private IEnumerator SpawnEnemyRoutine()
    {
        // Wait one frame so PortalSpawner.OnGetProduct (SetActiveWave) and Start()
        // (pool setup) have run, even on the pool's first-created portal where OnEnable
        // fires during Instantiate before the wave is stamped.
        yield return null;

        // Prefer wave-driven count/interval; fall back to the prefab's randomizers.
        WaveDefinition wave = m_portalProduct.ActiveWave;
        int amount = wave != null ? wave.EnemiesPerPortal : m_countRandomizer.RandomizeCount();
        LevelDefinition level = RunConfig.Current != null ? RunConfig.Current.Level : null;
        if (wave != null && level != null && level.EnemiesPerPortalOverride > 0) amount = level.EnemiesPerPortalOverride;
        float interval = wave != null ? wave.SpawnInterval : -1f;

        m_audioPlayer.Play(1);
        while (m_spawnCount < amount)
        {
            yield return new WaitForSeconds(interval > 0f ? interval : m_intervalRandomizer.RandomizeInterval());

            int prefabIndex = m_indexRandonmizer.GetRandomIndex();
            WaveEnemyEntry pickedEntry = null;

            if (wave != null && wave.Enemies != null && wave.Enemies.Count > 0)
            {
                pickedEntry = WaveEnemyPicker.PickEntry(wave.Enemies, Random.value);
                if (pickedEntry != null && pickedEntry.Enemy != null && pickedEntry.Enemy.Prefab != null)
                {
                    for (int i = 0; i < m_productPrefabs.Count; ++i)
                    {
                        if (m_productPrefabs[i] != null && m_productPrefabs[i].name == pickedEntry.Enemy.Prefab.name)
                        {
                            prefabIndex = i;
                            break;
                        }
                    }
                }
            }

            EnemyProduct enemy = Spawn<EnemyProduct>(prefabIndex);
            if (enemy != null)
            {
                ApplyEnemyStats(enemy, pickedEntry, wave);
            }

            ++m_spawnCount;
        }

        m_animator.SetTrigger("isClose");
        m_audioPlayer.PlayOneShot(2);
    }

    private void ApplyEnemyStats(EnemyProduct enemy, WaveEnemyEntry entry, WaveDefinition wave)
    {
        if (enemy == null) return;

        CharacterStatManager statManager = enemy.GetComponent<CharacterStatManager>();
        CharacterHP charHP = enemy.GetComponent<CharacterHP>();

        float healthScale = wave != null ? wave.HealthScale : 1f;
        float damageScale = wave != null ? wave.DamageScale : 1f;
        float speedScale = wave != null ? wave.SpeedScale : 1f;

        float hp = -1f;
        float damage = -1f;
        float speed = -1f;

        if (entry != null)
        {
            if (entry.CustomHealth > 0f) hp = entry.CustomHealth * healthScale;
            else if (entry.Enemy != null) hp = entry.Enemy.GetBaseStat(StatType.Health, 50f) * healthScale;

            if (entry.CustomDamage > 0f) damage = entry.CustomDamage * damageScale;
            else if (entry.Enemy != null) damage = entry.Enemy.GetBaseStat(StatType.Damage, 5f) * damageScale;

            if (entry.CustomSpeed > 0f) speed = entry.CustomSpeed * speedScale;
            else if (entry.Enemy != null) speed = entry.Enemy.GetBaseStat(StatType.MovementSpeed, 3f) * speedScale;
        }
        else if (wave != null && (wave.HealthScale != 1f || wave.DamageScale != 1f || wave.SpeedScale != 1f))
        {
            if (statManager != null)
            {
                if (statManager.StatDictionary.TryGetValue(StatType.Health, out float baseHp)) hp = baseHp * healthScale;
                if (statManager.StatDictionary.TryGetValue(StatType.Damage, out float baseDmg)) damage = baseDmg * damageScale;
                if (statManager.StatDictionary.TryGetValue(StatType.MovementSpeed, out float baseSpd)) speed = baseSpd * speedScale;
            }
        }

        if (statManager != null)
        {
            if (hp > 0f) statManager.SetStat(StatType.Health, hp);
            if (damage > 0f) statManager.SetStat(StatType.Damage, damage);
            if (speed > 0f) statManager.SetStat(StatType.MovementSpeed, speed);
        }

        if (charHP != null && hp > 0f)
        {
            charHP.ResetMaxHealth(hp);
        }
    }

    #region Override Spawner
    protected override void OnGetProduct(IProduct product)
    {
        EnemyProduct enemyProduct = ProductConverter.IProductToAnyType<EnemyProduct>(product);
        enemyProduct.transform.position = gameObject.transform.position;

        base.OnGetProduct(product);
    }

    #endregion

}
