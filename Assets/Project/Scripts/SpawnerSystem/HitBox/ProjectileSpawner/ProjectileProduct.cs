using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileProduct : DamageSource
{
    //
    [SerializeField] private float m_baseDamages;

    private Rigidbody2D m_projectileRigid;
    private Collider2D m_projectileCollider;

    private bool m_isReleased;

    public Rigidbody2D ProjectileRigid { get => m_projectileRigid; }

    public void SetData(HitBoxData data)
    {
        m_extraDamages = data.Damages;
        m_piercing = data.Piercing;
    }

    private void Awake()
    {
        m_projectileRigid = GetComponent<Rigidbody2D>();
        m_projectileCollider = GetComponent<Collider2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_isReleased = false;
    }

    protected override void Start()
    {
        m_lifeController.Subscribe(_ =>
        {
            if (!m_isReleased)
            {
                m_isReleased = true;
                m_pool.Release(this);
            }
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_isReleased) return;

        if (collision.gameObject.CompareTag(m_targetTag.ToString()))
        {
            DoDamage(collision);

            --m_piercing;
            //Debug.Log(m_piercing);
            if (m_piercing <= 0)
            {
                m_isReleased = true;
                m_pool.Release(this);
            }
        }
    }

    protected override void DoDamage(Collider2D targetCollider)
    {
        float totalDamages = m_baseDamages + m_extraDamages;
        targetCollider.GetComponent<BodyPart>().CharacterHP.GetDamages(totalDamages); 
    }
}
