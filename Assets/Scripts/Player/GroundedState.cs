using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : BaseState
{

    public override void StateStart(PlayerBehaviourStateMachine state, Collider2D collider, ContactFilter2D groundCheckFilter, RaycastHit2D ground, bool isGrounded)
    {
        //groundCheck(collider, groundCheckFilter, ground, isGrounded);
    }

    public override void StateUpdate(PlayerBehaviourStateMachine state)
    {
        Debug.Log("Hello from ground");
    }

    public override void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, float acceleration, Rigidbody2D rigidbody, Animator animator, float moveSpeed, RaycastHit2D ground, float animationStepSpeed, Transform transform, ref int direction, float groundFrictionDragMultiplier, Collider2D collider, ContactFilter2D groundCheckFilter, bool isGrounded)
    {   
        if(ground)
        {
        //groundCheck(collider, groundCheckFilter, ground, isGrounded);
        rigidbody.drag = ground.collider.friction * groundFrictionDragMultiplier;
        float moddedMoveSpeed = moveSpeed * (1 + rigidbody.drag * 0.1f);
        float moddedAcceleration = acceleration * (1 + rigidbody.drag * 0.1f);

        // Apply input to velocity to account for friction and drag
        if (moveInput.x > 0 && rigidbody.velocity.x < moddedMoveSpeed ||
             moveInput.x < 0 && rigidbody.velocity.x > -moddedMoveSpeed)
         {
             Vector2 forceToAdd = moveInput * moddedAcceleration * rigidbody.mass;
             forceToAdd = Vector3.ProjectOnPlane(forceToAdd, ground.normal); // Rotate the movement force to align with the ground normal
                
             rigidbody.AddForce(forceToAdd);

             // Clamp the velocity to the max speed only if force was added this update
             if (Mathf.Abs(rigidbody.velocity.x) > moveSpeed)
                 rigidbody.velocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -moveSpeed, moveSpeed), rigidbody.velocity.y);
         }
        else if (rigidbody.velocity.x > -0.5 && rigidbody.velocity.x < 0.5){
            state.StateSwitch(state.idleState);
        }

         // Decide the animation and direction of the character
         bool isOutsideDeadzone = Mathf.Abs(moveInput.x) > 0.1f;
         animator.SetBool("isRun", isOutsideDeadzone);
         animator.SetFloat("speed", Mathf.Abs(rigidbody.velocity.x) * animationStepSpeed / (moveSpeed));
         if (isOutsideDeadzone && Mathf.Sign(moveInput.x) != direction) // Only need to scale model if direction has changed
         {
             direction *= -1;
             transform.localScale = new Vector3(direction, 1, 1);
         }

        }



    }
}
