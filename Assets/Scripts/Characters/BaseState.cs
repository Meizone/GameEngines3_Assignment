using UnityEngine;

public abstract class BaseState
{
    public abstract void StateStart(PlayerBehaviourStateMachine state);

    public abstract void StateUpdate(PlayerBehaviourStateMachine state);

    public abstract void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, RaycastHit2D ground, ref int direction, playerStruct player, ref bool isGrounded);



}
