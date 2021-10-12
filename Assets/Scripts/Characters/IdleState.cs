using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public override void StateStart(PlayerBehaviourStateMachine state, Collider2D collider, ContactFilter2D groundCheckFilter, RaycastHit2D ground, bool isGrounded)
    {
        
    }

    public override void StateUpdate(PlayerBehaviourStateMachine state)
    {
        //Debug.Log("Hello from idle");
    }

    public override void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, RaycastHit2D ground, ref int direction,playerStruct player)
    {
        if(moveInput.x != 0)
        {
            state.StateSwitch(state.groundState);
        }
    }


}
