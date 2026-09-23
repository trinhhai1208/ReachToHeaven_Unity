using UnityEngine;
using UnityEngine.Pool;

public class PortalProduct : MonoBehaviour, IProduct
{
    //
    private ObjectPool<IProduct> m_pool;

    public Transform EnemyField { get; private set; }
    public Rigidbody2D PlayerRigidbody { get; private set; }

    ///<summary>Wave this portal must spawn for (set by PortalSpawner before activation).</summary>
    public WaveDefinition ActiveWave { get; private set; }

    public void Init(Rigidbody2D playerRigidbody, Transform enemyProductField)
    {
        PlayerRigidbody = playerRigidbody;
        EnemyField = enemyProductField;
    }

    public void SetActiveWave(WaveDefinition wave)
        => ActiveWave = wave;

    public void Release()
        => m_pool.Release(this);

    #region Implement IProduct
    public void SetPool(ObjectPool<IProduct> pool)
        => m_pool = pool;
    public ObjectPool<IProduct> GetPool()
        => m_pool;
    #endregion

}
