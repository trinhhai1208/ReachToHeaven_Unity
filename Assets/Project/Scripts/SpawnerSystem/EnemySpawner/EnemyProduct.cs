using UnityEngine;
using UnityEngine.Pool;

public class EnemyProduct : MonoBehaviour, IProduct
{
    //
    private ObjectPool<IProduct> m_pool;

    public Rigidbody2D PlayerRigidbody { get; private set; }

    public void Init(Rigidbody2D playerRigidbody)
        => PlayerRigidbody = playerRigidbody;

    private void OnEnable() => EnemyTracker.OnActivated();
    private void OnDisable() => EnemyTracker.OnDeactivated();

    #region Implement IProduct
    public ObjectPool<IProduct> GetPool()
        => m_pool;
    public void SetPool(ObjectPool<IProduct> pool)
        => m_pool = pool;
    #endregion
}
