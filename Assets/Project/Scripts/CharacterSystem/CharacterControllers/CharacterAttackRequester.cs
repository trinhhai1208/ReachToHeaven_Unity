using UnityEngine;

//==========================DEFINE BEHAVIOUR OF ATTACKSENSOR & SETUP STATIC CONTEXT: AnimatorController=================================

[RequireComponent(typeof(AnimatorController))]
public abstract class CharacterAttackRequester : MonoBehaviour, IStateRequester
{
    //
    protected NextStateChecker m_stateChecker;

    ///<summary>
    ///Get NextStateChecker's reference
    /// </summary>
    protected virtual void Start()
    {
        m_stateChecker = GetComponent<CharacterController>().StateMachine.StateChecker;
    }

    ///<summary>
    ///Setup AnimatorController, RoutineCaller and CompleteEvent
    /// </summary>
    public virtual void SetupBasicStaticContext(AttackStateContext attackContext)
    {
        attackContext.CharacterAnimatorController = GetComponent<AnimatorController>();

        attackContext.RoutineCaller = this;
        attackContext.CompleteEvent = m_stateChecker.ResetState;
    }

    #region Implement IStateRequester
    public abstract void RequestState();
    public abstract void SetupStaticContext();
    public abstract void SetupDynamicContext();
    #endregion
}
