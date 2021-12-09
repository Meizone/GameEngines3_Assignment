using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : BaseState
{

    public override void StateStart(PlayerBehaviourStateMachine state)
    {
        //groundCheck(collider, groundCheckFilter, ground, isGrounded);
    }

    public override void StateUpdate(PlayerBehaviourStateMachine state)
    {

    }

    public override void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, RaycastHit2D ground, ref int direction, playerStruct player, ref bool isGrounded , ref bool isJump)
    {
        if (ground.collider == null)
        {
            state.StateSwitch(state.idleState);
            return;
        }

        // State Switching
        if (!player.animator.GetBool("isRun"))
        {
            state.StateSwitch(state.idleState);
        }

        player.rigidbody.drag = ground.collider.friction * player.groundFrictionDragMultiplier;
        float moddedMoveSpeed = player.moveSpeed * (1 + player.rigidbody.drag * 0.1f);
        float moddedAcceleration = player.acceleration * (1 + player.rigidbody.drag * 0.1f);

            // Apply input to velocity to account for friction and drag
        if (moveInput.x > 0 && player.rigidbody.velocity.x < moddedMoveSpeed ||
                moveInput.x < 0 && player.rigidbody.velocity.x > -moddedMoveSpeed)
        {
            Vector2 forceToAdd = moveInput * moddedAcceleration * player.rigidbody.mass;
            forceToAdd = Vector3.ProjectOnPlane(forceToAdd, ground.normal); // Rotate the movement force to align with the ground normal

            player.rigidbody.AddForce(forceToAdd);

            // Clamp the velocity to the max speed only if force was added this update
            if (Mathf.Abs(player.rigidbody.velocity.x) > player.moveSpeed)
                player.rigidbody.velocity = new Vector2(Mathf.Clamp(player.rigidbody.velocity.x, -player.moveSpeed, player.moveSpeed), player.rigidbody.velocity.y);
        }


        Jump(player, ref isJump);

        // Decide the animation and direction of the character
        bool isOutsideDeadzone = Mathf.Abs(moveInput.x) > 0.2f;
        player.animator.SetBool("isRun", isOutsideDeadzone);
        player.animator.SetFloat("speed", Mathf.Abs(player.rigidbody.velocity.x) * player.animationStepSpeed / (player.moveSpeed));

        if (isOutsideDeadzone && Mathf.Sign(moveInput.x) != direction) // Only need to scale model if direction has changed
        {
            direction *= -1;
            player.transform.localScale = new Vector3(direction, 1, 1);
        }

        



    }
}
