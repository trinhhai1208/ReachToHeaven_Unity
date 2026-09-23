using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterHP), typeof(CharacterController))]
public class CharacterHurtRequester : MonoBehaviour, IStateRequester
{
    //
    [FormerlySerializedAs("m_invinsibleTime")]
    [SerializeField] private float m_invincibleTime;
    [SerializeField] private AudioClip m_hurtSFX;
    [SerializeField] private AudioClip m_deathSFX;
    [SerializeField] private bool m_spawnGemOnDeath = true;
    [SerializeField] private bool m_screenShakeOnHurt = false;
 
    private NextStateChecker m_stateChecker;
    private CharacterHP m_characterHP;
    private bool m_isDeath = false;

    private RequestStateData<HurtStateContext> m_requestHurtData = 
        new RequestStateData<HurtStateContext>(StateType.HurtState);
    private RequestStateData<DeathStateContext> m_requestDeathData =
        new RequestStateData<DeathStateContext>(StateType.DeathState);

    [SerializeField] private ComponentCollector m_componentCollector = new ComponentCollector();

    private void Awake()
    {
        m_characterHP = GetComponent<CharacterHP>();
    }

    private void Start()
    {
        m_stateChecker = GetComponent<CharacterController>().StateMachine.StateChecker;
        SetupStaticContext();

        m_characterHP.Subscribe(RequestState);
        m_characterHP.OnDeath += () => m_isDeath = true;
        m_characterHP.OnDeath += () =>
        {
            if (m_spawnGemOnDeath && GemManager.Instance != null)
                GemManager.Instance.WrappedSpawn(gameObject.transform.position);
        };
        // Count a kill only on genuine death, so despawn/scene teardown don't inflate the stat.
        if (TryGetComponent(out EnemyProduct _))
            m_characterHP.OnDeath += EnemyTracker.OnKilled;
    }

    private void OnEnable()
        => m_isDeath = false;

    #region implement istaterequester
    public void RequestState()
    {
        SetupDynamicContext();

        if (m_isDeath) m_stateChecker.RequestHandle(m_requestDeathData);
        else m_stateChecker.RequestHandle(m_requestHurtData);
    }
    public void SetupStaticContext()
    {
        AnimatorController animController = GetComponent<AnimatorController>();

        m_requestHurtData.Context = new HurtStateContext();
        m_requestHurtData.Context.CharacterAnimatorController = animController;
        m_requestHurtData.Context.RoutineCaller = this;

        if(m_hurtSFX != null)
            m_requestHurtData.Context.EnterEvent = () => EventAudioManager.Instance.PlayEventSFX(m_hurtSFX);

        if (m_screenShakeOnHurt)
            m_requestHurtData.Context.EnterEvent += () =>
                Camera.main?.transform.DOShakePosition(0.2f, 0.2f, 10, 90, false, true);

        m_requestHurtData.Context.CompleteEvent = m_stateChecker.ResetState;

        m_requestDeathData.Context = new DeathStateContext();
        m_requestDeathData.Context.CharacterAnimatorController = animController;
        m_requestDeathData.Context.RoutineCaller = this;

        m_requestDeathData.Context.EnterEvent += m_componentCollector.DeactiveAllComponents;
        if (m_deathSFX != null)
            m_requestDeathData.Context.EnterEvent += () => EventAudioManager.Instance.PlayEventSFX(m_deathSFX);
        m_requestDeathData.Context.CompleteEvent += m_stateChecker.ResetState;
        m_requestDeathData.Context.DecayEvent += m_componentCollector.ActiveAllComponents;
        m_requestDeathData.Context.DecayEvent += Decay;
    }

    ///<summary>
    ///Return the object to its ObjectPool when possible, otherwise just deactivate it.
    /// </summary>
    private void Decay()
    {
        if (TryGetComponent(out EnemyProduct enemyProduct) && enemyProduct.GetPool() != null)
            enemyProduct.GetPool().Release(enemyProduct);
        else
            gameObject.SetActive(false);
    }
    public void SetupDynamicContext()
    {
        
    }
    #endregion
}
