using UnityEngine;
using System.Collections.Generic;

public class Altar : MonoBehaviour
{
    //
    [Tooltip("Dectect only once in the beginning of the game")]
    [SerializeField] private float m_detectRange;
    [SerializeField] private LayerMask m_detectLayer;
    [SerializeField] List<SpriteRenderer> m_runes = new List<SpriteRenderer>();

    private Animator m_animator;

    private void Awake()
        => m_animator = GetComponent<Animator>();

    private void Start()
    {
        Collider2D collider = Physics2D.OverlapCircle(gameObject.transform.position, m_detectRange, m_detectLayer);

        bool isFadeIn = true;
        if (collider != null && collider.gameObject.CompareTag("Player"))
            isFadeIn = false;

        m_animator.SetBool("isFadeOut", isFadeIn);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            m_animator.SetBool("isFadeOut", false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            m_animator.SetBool("isFadeOut", true);
    }

    #region Test
    [SerializeField] private bool m_showDetectRange;

    private void OnDrawGizmosSelected()
    {
        if (m_showDetectRange)
            Gizmos.DrawWireSphere(gameObject.transform.position, m_detectRange);
    }
    #endregion
}
