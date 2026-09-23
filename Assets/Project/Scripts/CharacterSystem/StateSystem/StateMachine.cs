using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
///<summary>
///Note: Requiring sensors component
/// </summary>
public class StateMachine
{
    // 
    #region StateDictionary
    [SerializeField] private List<CharacterState> m_characterStates = new List<CharacterState>();

    private Dictionary<StateType, CharacterState> m_stateDict = new Dictionary<StateType, CharacterState>();
    public Dictionary<StateType, CharacterState> StateDict { get => m_stateDict; }
    #endregion

    #region Supporter
    private NextStateChecker m_stateChecker = new NextStateChecker();
    private StateContextLoader m_stateContextLoader = new StateContextLoader();
    public NextStateChecker StateChecker { get => m_stateChecker; }
    #endregion

    private string m_gameObjectName;

    public CharacterState CurrentState { get; private set; }

    public void Init(string name, AnimatorController animatorController)
    {
        MappingState();

        m_gameObjectName = name;

        m_stateChecker.Init(name, this, animatorController);

        CurrentState = m_stateDict[StateType.IdleState];
    }

    private void MappingState()
    {
        foreach (var state in m_characterStates)
        {
            if (m_stateDict.ContainsKey(state.Type))
                Debug.LogWarning($"{this}: A StateLogic in StateDictionary is overrided");

            m_stateDict[state.Type] = state;
        }
    }

    public void ChangeState<TStateContext>(RequestStateData<TStateContext> nextState)
        where TStateContext : StateContext
    {
        CurrentState.Logic.ExitState(m_stateContextLoader.GetContext(CurrentState.Type));
        m_stateContextLoader.LoadContext(nextState.Context, nextState.Type);
        CurrentState = m_stateDict[nextState.Type];
        CurrentState.Logic.EnterState(m_stateContextLoader.GetContext(nextState.Type));
    }
}
