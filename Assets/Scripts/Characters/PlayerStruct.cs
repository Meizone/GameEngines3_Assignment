using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public struct playerStruct
{
    public float acceleration;
    public Rigidbody2D rigidbody; 
    public Animator animator; 
    public float moveSpeed; 
    public float animationStepSpeed; 
    public Transform transform; 
    public float groundFrictionDragMultiplier; 
    public Collider2D collider; 
    public ContactFilter2D groundCheckFilter; 



    public playerStruct(float accel, Rigidbody2D r, Animator ani, float move, float stepSpeed, Transform player_transform, float groundFriction, Collider2D collider2d, ContactFilter2D groundFilter) : this()
    {
        acceleration = accel;
        rigidbody = r;
        animator = ani;
        moveSpeed = move;
        animationStepSpeed = stepSpeed;
        transform = player_transform;
        groundFrictionDragMultiplier = groundFriction;
        collider = collider2d;
        groundCheckFilter = groundFilter;

    }

}