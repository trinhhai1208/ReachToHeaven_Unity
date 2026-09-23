using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputController))]
public class PlayerAttackRequester : CharacterAttackRequester
{
    //
    [SerializeField] private ProjectileSpawner m_projectileSpawner;
    [SerializeField] private List<AudioClip> m_clips = new List<AudioClip>();
    private InputController m_inputController;
    private CharacterStatManager m_statManager;
    private Camera m_mainCamera;

    private RequestStateData<PlayerAttackStateContext> m_requestData =
        new RequestStateData<PlayerAttackStateContext>(StateType.AttackState);

    private HitBoxData m_projectileData = new HitBoxData();

    #region Suppoter
    private AttackCountDownTimer m_attackTime = new AttackCountDownTimer();
    private AttackEventHandler m_eventHandler = new AttackEventHandler();
    private WeaponSwitcher m_weaponSwitcher = new WeaponSwitcher();
    #endregion

    private void Awake()
    {
        m_inputController = GetComponent<InputController>();
        m_statManager = GetComponent<CharacterStatManager>();
        m_mainCamera = Camera.main;

        m_eventHandler.OnEventCallBack +=
            () =>
            {
                float damages = m_statManager.StatDictionary[StatType.Damage];
                if (Random.value <= m_statManager.StatDictionary[StatType.CritChance] / 100)
                    damages = damages + (m_statManager.StatDictionary[StatType.CritDamage] * damages);

                m_projectileData.Damages = damages;
                m_projectileData.Piercing = (int)m_statManager.StatDictionary[StatType.Piercing];
                m_projectileSpawner.SetData(m_requestData.Context.Direction, m_projectileData);

                m_projectileSpawner.Spawn<ProjectileProduct>(m_weaponSwitcher.WeaponIndex);

                EventAudioManager.Instance.PlayEventSFX(m_clips[m_weaponSwitcher.WeaponIndex]);
            };
    }

    protected override void Start()
    {
        base.Start();
        SetupStaticContext();

        m_weaponSwitcher.Init(m_inputController, m_projectileSpawner.GetListCount());

        m_inputController.Subscribe(OnAttack);
    }

    private void OnDestroy()
    {
        m_inputController.UnSubscribe(OnAttack);
        m_weaponSwitcher.Dispose();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (m_attackTime.IsReadyToAttack())
        {
            RequestState();
            m_eventHandler.ResetAnimationTime();
            m_attackTime.StartCountDown(m_statManager.StatDictionary[StatType.AttackCountDown]);
        }
    }

    #region Call in Animation Event
    public void WrappedSpawnProjectile(float animationTime)
    {
        m_eventHandler.Spawn(animationTime);
    }
    #endregion

    #region Override CharacterAttackSensor
    public override void RequestState()
    {
        SetupDynamicContext();
        m_stateChecker.RequestHandle(m_requestData);
    }

    public override void SetupStaticContext()
    {
        m_requestData.Context = new PlayerAttackStateContext();
        SetupBasicStaticContext(m_requestData.Context);
    }

    public override void SetupDynamicContext()
    {
        // Refresh the cache if the previously cached camera was destroyed (e.g. scene change).
        if (m_mainCamera == null) m_mainCamera = Camera.main;

        Vector2 direction = m_mainCamera.ScreenToWorldPoint(Mouse.current.position.value) - gameObject.transform.position;
        m_requestData.Context.Direction = direction.normalized;

        m_requestData.Context.WeaponIndex = m_weaponSwitcher.WeaponIndex;
    }
    #endregion
}
