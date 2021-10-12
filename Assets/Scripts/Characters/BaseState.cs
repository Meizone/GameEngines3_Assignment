using UnityEngine;

public abstract class BaseState
{
    public abstract void StateStart(PlayerBehaviourStateMachine state, Collider2D collider, ContactFilter2D groundCheckFilter, RaycastHit2D ground, bool isGrounded);

    public abstract void StateUpdate(PlayerBehaviourStateMachine state);

    public abstract void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, RaycastHit2D ground, ref int direction,playerStruct player);



}
