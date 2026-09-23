using UnityEngine;

public class HitBoxSpawner : Spawner
{
    //
    [SerializeField] private CharacterStatManager m_statManager;
    [SerializeField] private float m_attackRadius;
    private float m_scaledAttackRadius;
    private void Start()
    {
        SetupProductPools();
    }

    protected override void OnGetProduct(IProduct product)
    {
        base.OnGetProduct(product);
        if (product is HitBoxProduct hitBox)
        {
            hitBox.SetExtraDamages(m_statManager.StatDictionary[StatType.Damage]);
            hitBox.SetHitBoxRadius(m_attackRadius);
            hitBox.gameObject.transform.position = gameObject.transform.position;
        }
    }

    #region Test
    private void OnDrawGizmosSelected()
    {
        m_scaledAttackRadius = m_attackRadius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        Gizmos.DrawWireSphere(gameObject.transform.position, m_scaledAttackRadius);
    }
    #endregion
}
