using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public override void StateStart(PlayerBehaviourStateMachine state)
    {

    }

    public override void StateUpdate(PlayerBehaviourStateMachine state)
    {
        //Debug.Log("Hello from idle");
    }

    public override void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, RaycastHit2D ground, ref int direction, playerStruct player, ref bool isGrounded, ref bool isJump)
    {
        if (moveInput.x != 0 && isGrounded)
        {
            state.StateSwitch(state.groundState);
        }
    }


}

