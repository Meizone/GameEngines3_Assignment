using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBehaviourStateMachine : MonoBehaviour
{
    // Referenced variables
    private Rigidbody2D rigidbody;
    private Animator animator;
    private Collider2D collider;
    private IEnumerator EncounterGenerator;


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


    [Header("Battle Handler")]
    [SerializeField, Min(0)]
    private float battleRate;
    [SerializeField]
    private LevelMetaData sceneMetaData;
    [SerializeField]
    private static bool previousBattle = true;
    [SerializeField]
    private bool inBattle = false;


    [Header("Ground Checking")]
    [SerializeField]
    private ContactFilter2D groundCheckFilter;

    // Private variables
    private Vector2 moveInput = Vector2.zero;
    private bool IsJump = false;
    public int direction = 1;
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
        player = new playerStruct(acceleration, rigidbody, animator, moveSpeed, animationStepSpeed, transform, groundFrictionDragMultiplier, collider, groundCheckFilter);
        currentState.StateStart(this);

        PlayerLocationLoad.LoadLocation(this);


    }

    private void OnMove(InputValue value)
    {
        if(!inBattle)
            moveInput = value.Get<Vector2>();
        else
            moveInput = new Vector2(0f,0f);
    }

    // Check if the value of isJump is not equal to 0, If true return true, if not, return false.
    private void OnJump(InputValue value)
    {
        IsJump = ((value.Get<float>() != 0) ? true : false); 
    }


    // Update is called once per frame
    void Update()
    {
        currentState.StateUpdate(this);
        Debug.Log(previousBattle);
    }

    void FixedUpdate()
    {
        groundCheck();
        currentState.StateFixedUpdate(this, moveInput, ground, ref direction, player, ref isGrounded, ref IsJump);
    }

    public void StateSwitch(BaseState state)
    {
        currentState = state;
        state.StateStart(this);
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


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Encounter")
        {

            if(EncounterGenerator != null)
                StopCoroutine(EncounterGenerator);

            EncounterGenerator = EncounterCheck();
            StartCoroutine(EncounterGenerator);
        }

        // Prelimenary, StartCoroutine to loop a random number generator
        // If number is below a certain threshhold (Set in Player) The player
        // Will be sent to the instance
        
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "Encounter")
        {
            StopCoroutine(EncounterGenerator);
            // Previous Battle is a static bool to check if the player was previously in a battle or not.
            previousBattle = true;
            // Prelimenary, StopCoroutine of Looping Random Number Generator
            // Will stop the generation of Encounters
        }
    }



    private IEnumerator EncounterCheck()
    {
        while(true)
        {
            for(;;)
            {
                int rng = Random.Range(0,101);
                Debug.Log(rng);
                if(rng <= battleRate && previousBattle)
                {
                    PlayerLocationLoad.SaveLocation(this);
                    LevelLoader.LoadLevel(sceneMetaData);
                    Debug.Log("Battle!");
                    previousBattle = false;
                    inBattle = true;
                    break;
                }
                yield return new WaitForSeconds(1.0f);
            }
            break;
        }

    }


}
