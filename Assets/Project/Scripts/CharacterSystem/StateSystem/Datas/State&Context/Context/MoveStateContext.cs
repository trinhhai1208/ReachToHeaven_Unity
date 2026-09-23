using UnityEngine;

public class MoveStateContext : StateContext
{
    //Static Data
    public Rigidbody2D CharacterRigid;
    public MonoBehaviour RoutineCaller;
    public Coroutine MoveRoutine;

    //DynamicData
    public Vector2 TargetPosition;
    public float MovementSpeed;
    public int SpeedParameter = 0;

    public MoveStateContext(AnimatorController animatorController, Rigidbody2D characterRigid)
    {
        CharacterAnimatorController = animatorController;
        CharacterRigid = characterRigid;
    }
}
