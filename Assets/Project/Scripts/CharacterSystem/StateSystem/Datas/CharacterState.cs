using System;
using UnityEngine;

[Serializable]
public struct CharacterState
{
    //
    public int Priority;
    public StateType Type;
    public StateLogic Logic; 
}
