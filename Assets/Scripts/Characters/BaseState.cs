using UnityEngine;

public abstract class BaseState
{
    public abstract void StateStart(PlayerBehaviourStateMachine state, Collider2D collider, ContactFilter2D groundCheckFilter, RaycastHit2D ground, bool isGrounded);

    public abstract void StateUpdate(PlayerBehaviourStateMachine state);

    public abstract void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, float acceleration, Rigidbody2D rigidbody, Animator animator, float moveSpeed, RaycastHit2D ground, float animationStepSpeed, Transform transform, ref int direction , float groundFrictionDragMultiplier, Collider2D collider, ContactFilter2D groundCheckFilter, bool isGrounded);



}
