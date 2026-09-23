using System;
using UnityEngine;

[Serializable]
public class AttackCountDownTimer
{
    //
    private float m_nextAttackTime;

    public bool IsReadyToAttack() => Time.time >= m_nextAttackTime;

    public void StartCountDown(float countDownTime)
        => m_nextAttackTime = Time.time + countDownTime;
}
