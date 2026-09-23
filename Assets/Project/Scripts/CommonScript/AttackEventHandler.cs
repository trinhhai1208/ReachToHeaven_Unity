using System;
using UnityEngine;

public class AttackEventHandler
{
    //
    private float m_currentAnimationTime;

    public event Action OnEventCallBack;

    public void ResetAnimationTime() => m_currentAnimationTime = -1;

    #region Call in AnimationEvent
    public void Spawn(float animationTime)
    {
        if (animationTime != m_currentAnimationTime)
        {
            OnEventCallBack?.Invoke();
            m_currentAnimationTime = animationTime;
        }
    }
    #endregion
}
