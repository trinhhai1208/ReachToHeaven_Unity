using UnityEngine;

public class EnemyMovementRequester : CharacterMovementRequester
{
    //
    [SerializeField] protected CharacterStatManager m_statManager;
    [SerializeField] protected float m_distanceThreshold;
    protected EnemyProduct m_enemyProduct;

    protected override void Awake()
    {
        base.Awake();
        m_enemyProduct = GetComponent<EnemyProduct>();
    }

    ///<summary>
    ///Check if enemy reachs target and requests state or not
    /// </summary>
    protected virtual void LateUpdate()
    {
        float distSqr = ((Vector2)m_enemyProduct.PlayerRigidbody.position
            - (Vector2)gameObject.transform.position).sqrMagnitude;
        if (distSqr > m_distanceThreshold * m_distanceThreshold)
            RequestState();
        else if (!m_isComplete)
        {
            m_nextStateChecker.ResetState();
            m_isComplete = true;
        }
    }

    #region Override CharacterMovementRequester
    public override void SetupDynamicContext()
    {
        m_requestData.Context.TargetPosition = m_enemyProduct.PlayerRigidbody.position;
        m_requestData.Context.MovementSpeed = m_statManager.StatDictionary[StatType.MovementSpeed];
    }
    #endregion
}
