using UnityEngine;

public abstract class BaseState
{
    public abstract void StateStart(PlayerBehaviourStateMachine state);

    public abstract void StateUpdate(PlayerBehaviourStateMachine state);

    public abstract void StateFixedUpdate(PlayerBehaviourStateMachine state, Vector2 moveInput, RaycastHit2D ground, ref int direction, playerStruct player, ref bool isGrounded, ref bool isJump);


    public void Jump(playerStruct player, ref bool isJump)
    {
        if(isJump == true)
        {
            Debug.Log("Jumped");
            player.rigidbody.velocity = (Vector2.up * 20) + player.rigidbody.velocity;
            isJump = false;
        }
    }

}
