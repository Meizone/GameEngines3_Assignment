using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacterController : MonoBehaviour
{
    // Referenced variables
    private Rigidbody2D rigidbody;
    private Animator animator;
    private Collider2D collider;

    // Serialized variables
    [Header("Movement and Acceleration")]
    [SerializeField, Min(0)]
    private float moveSpeed;
    [SerializeField, Min(0)]
    private float acceleration;
    [SerializeField, Min(0)]
    private float groundFrictionDragMultiplier;
    [SerializeField]
    private float animationStepSpeed;

    [Header("Ground Checking")]
    [SerializeField]
    private ContactFilter2D groundCheckFilter;

    // Private variables
    private Vector2 moveInput = Vector2.zero;
    private int direction = 1;
    [SerializeField]
    private bool isGrounded;
    public bool grounded { get { return isGrounded; } }
    private RaycastHit2D ground;

    // Constants
    private const float inputAnimationDeadzone = 0.1f;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        CheckGrounded();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (isGrounded)
        {
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

            // Decide the animation and direction of the character
            bool isOutsideDeadzone = Mathf.Abs(moveInput.x) > inputAnimationDeadzone;
            animator.SetBool("isRun", isOutsideDeadzone);
            animator.SetFloat("speed", Mathf.Abs(rigidbody.velocity.x) * animationStepSpeed / (moveSpeed));
            if (isOutsideDeadzone && Mathf.Sign(moveInput.x) != direction) // Only need to scale model if direction has changed
            {
                direction *= -1;
                transform.localScale = new Vector3(direction, 1, 1);
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded = false; // Assume not grounded until otherwise proven.
        
        RaycastHit2D[] hits = new RaycastHit2D[1];
        collider.Cast(Vector2.down, groundCheckFilter, hits, 0.1f);

        if (hits[0]) // If there was, in fact, a hit.
        {
            isGrounded = true;
            ground = hits[0];
        }
    }
}
