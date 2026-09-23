using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyAttackRange : MonoBehaviour
{
    //
    [SerializeField] private LayerMask m_targetLayerMask;
    [SerializeField] private CharacterStatManager m_characterStatManager;
    private CircleCollider2D m_attackRange;
    private Coroutine m_cachedDetectRoutine;

    private AttackCountDownTimer m_attackTimer = new AttackCountDownTimer();

    public event Action OnInRange;
    public event Action OnOutRange;

    private void Awake()
    {
        m_attackRange = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (m_attackTimer.IsReadyToAttack())
            {
                OnInRange?.Invoke();
                m_attackTimer.StartCountDown(m_characterStatManager.StatDictionary[StatType.AttackCountDown]);
            }
            m_cachedDetectRoutine = StartCoroutine(DetectPlayerRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (m_cachedDetectRoutine != null) StopCoroutine(m_cachedDetectRoutine);

            Collider2D collider = 
                Physics2D.OverlapCircle(gameObject.transform.position, m_attackRange.radius, m_targetLayerMask);

            if (collider == null) OnOutRange?.Invoke();
        }
    }

    private IEnumerator DetectPlayerRoutine()
    {
        while(true)
        {
            yield return new WaitUntil(m_attackTimer.IsReadyToAttack);

            Collider2D collider =
                    Physics2D.OverlapCircle(gameObject.transform.position, m_attackRange.radius, m_targetLayerMask);

            if (collider != null)
            {
                OnInRange?.Invoke();
                m_attackTimer.StartCountDown(m_characterStatManager.StatDictionary[StatType.AttackCountDown]);
            }
        }
    }
}
