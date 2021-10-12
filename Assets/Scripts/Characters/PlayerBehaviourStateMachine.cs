using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class PlayerBehaviourStateMachine : MonoBehaviour
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
    private bool isGrounded = false;
    public bool grounded { get { return isGrounded; } }
    private RaycastHit2D ground;

    // Constants
    private const float inputAnimationDeadzone = 0.1f;

    //States
    BaseState currentState;
    public IdleState idleState = new IdleState();
    public GroundedState groundState = new GroundedState();
    playerStruct player;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();

        currentState = groundState;
        player = new playerStruct(acceleration, rigidbody, animator, moveSpeed, animationStepSpeed, transform, groundFrictionDragMultiplier, collider, groundCheckFilter, isGrounded);
        currentState.StateStart(this, collider, groundCheckFilter, ground, isGrounded);


    }


    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    // Update is called once per frame
    void Update()
    {
        currentState.StateUpdate(this);
    }

    void FixedUpdate()
    {
        groundCheck();
        currentState.StateFixedUpdate(this, moveInput, ground, ref direction, player);
    }

    public void StateSwitch(BaseState state)
    {
        currentState = state;
        state.StateStart(this,collider, groundCheckFilter, ground, isGrounded);
    }

    public void groundCheck()
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
