using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    Rigidbody2D theRB;
    Animator theAnim;
    SpriteRenderer theSR;

    private PlayerControlls playerControlls;

    [SerializeField]
    private InputAction[] inputActions;

    private void InstantiateInputActions()
    {
        inputActions = new InputAction[]
        {
            playerControlls.Player.Move,
            playerControlls.Player.JumpUp,
            playerControlls.Player.JumpDown,
            playerControlls.Player.JumpRight,
            playerControlls.Player.JumpLeft,
            playerControlls.Player.TouchPosition,
            playerControlls.Player.TouchPress
        };
    }

    private void OnDisable()
    {
        foreach (var action in inputActions)
        {
            action.Disable();
        }
    }

    private void Start()
    {
        InstantiateInputActions();

        foreach (var action in inputActions)
        {
            action.Enable();
            AttachActionMethod(action);
        }
        if (TouchDetection.instance == null)
        {
            return;
        }
        TouchDetection.instance.OnSwipe += swipeDirection =>
        {
            JumpWithSwipe(swipeDirection);
        };
        TouchDetection.instance.OnPress += () =>
        {
            JumpUp(default);
        };
    }

    private void JumpWithSwipe(Vector2 swipeDirection)
    {
        if (swipeDirection.x > 0)
        {
            JumpRight(default);
        }
        else if (swipeDirection.x < 0)
        {
            JumpLeft(default);
        }
        else if (swipeDirection.y > 0)
        {
            JumpUp(default);
        }
        else if (swipeDirection.y < 0)
        {
            JumpDown(default);
        }
    }

    private void AttachActionMethod(InputAction action)
    {
        switch (action.name)
        {
            case "JumpUp":
                action.performed += JumpUp;
                break;
            case "JumpDown":
                action.performed += JumpDown;
                break;
            case "JumpRight":
                action.performed += JumpRight;
                break;
            case "JumpLeft":
                action.performed += JumpLeft;
                break;
            // Add more cases if needed
        }
    }

    [SerializeField]
    private float moveSpeed,
        upJumpForce = 7f,
        downJumpForce = 4f,
        sideJumpForce = 10f;

    public enum PlayerControlState
    {
        PreGame,
        Gameplay,
        PostGame
    }

    private PlayerControlState controlState = PlayerControlState.PreGame;

    private float scrollJumpBuffer = 0f;

    [SerializeField]
    private float maxScrollJumpBuffer = 0.02f;
    const string JUMP_ANIMATION = "Jump",
        DOWNJUMP_ANIMATION = "DownJump",
        FALL_ANIMATION = "Fall";
    private string currentState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerControlls = new PlayerControlls();
        theAnim = GetComponent<Animator>();
        theSR = GetComponent<SpriteRenderer>();
        theRB = GetComponent<Rigidbody2D>();
        if (controlState == PlayerControlState.PreGame)
        {
            theRB.gravityScale = 0f;
            theRB.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (controlState != PlayerControlState.Gameplay)
        {
            // Do not allow actions in states other than "Gameplay"
            return;
        }
        if (other.gameObject.CompareTag("Scissors"))
        {
            GameManager.instance.EndGame();
            //TODO:
            // 1. Play death animation
            // 2. Play death sound
            // 3. Play game over sound
        }
        if (other.gameObject.CompareTag("Stone"))
        {
            Destroy(other.gameObject);
            ScoreManager.instance.IncreasePoints();
        }
    }

    private void StartVelocityUpdate(Vector2 velocity)
    {
        theRB.velocity = velocity;
    }

    private void SetControlState(PlayerControlState newState)
    {
        controlState = newState;
    }

    public void StartPlayerGameplay()
    {
        instance.transform.position = Vector3.zero;
        SetControlState(PlayerControlState.Gameplay);
        theRB.gravityScale = 1f;
        theRB.velocity = Vector2.up;
        // Perform any necessary initialization for gameplay here
    }

    public void EndPlayerGameplay()
    {
        SetControlState(PlayerControlState.PostGame);
        // Perform any necessary initialization for gameplay here
    }

    private void JumpUp(InputAction.CallbackContext context)
    {
        if (controlState != PlayerControlState.Gameplay)
        {
            // Do not allow actions in states other than "Gameplay"
            return;
        }
        if (true || context.performed) //TODO: remove true
        {
            if (context.ReadValue<float>() < 5)
            {
                scrollJumpBuffer = 0f;
                float currentVelocityX = theRB.velocity.x;
                StartVelocityUpdate(new Vector2(currentVelocityX * 0.5f, upJumpForce));

                if (IsAnimationPlayingAndUpdateCurrentState(FALL_ANIMATION))
                {
                    ChangeAnimationState(JUMP_ANIMATION);
                }
            }
            // this is a scroll jump
            else if (scrollJumpBuffer <= 0)
            {
                scrollJumpBuffer = maxScrollJumpBuffer;
                float currentVelocityX = theRB.velocity.x;
                StartVelocityUpdate(new Vector2(currentVelocityX * 0.5f, upJumpForce));
                if (IsAnimationPlayingAndUpdateCurrentState(FALL_ANIMATION))
                {
                    ChangeAnimationState(JUMP_ANIMATION);
                }
            }
            else if (scrollJumpBuffer > 0)
            {
                scrollJumpBuffer -= Time.deltaTime;
            }
        }
    }

    private void JumpDown(InputAction.CallbackContext context)
    {
        if (controlState != PlayerControlState.Gameplay)
        {
            // Do not allow actions in states other than "Gameplay"
            return;
        }
        if (true || context.performed) //TODO: remove true
        {
            if (context.ReadValue<float>() < 5)
            {
                scrollJumpBuffer = 0f;
                StartVelocityUpdate(Vector2.down * downJumpForce);
                if (IsAnimationPlayingAndUpdateCurrentState(FALL_ANIMATION))
                {
                    ChangeAnimationState(DOWNJUMP_ANIMATION);
                }
            }
            // this is a scroll jump
            else if (scrollJumpBuffer <= 0)
            {
                scrollJumpBuffer = maxScrollJumpBuffer;
                StartVelocityUpdate(Vector2.down * downJumpForce);
                if (IsAnimationPlayingAndUpdateCurrentState(FALL_ANIMATION))
                {
                    ChangeAnimationState(DOWNJUMP_ANIMATION);
                }
            }
            else if (scrollJumpBuffer > 0)
            {
                scrollJumpBuffer -= Time.deltaTime;
            }
        }
    }

    private void JumpLeft(InputAction.CallbackContext context)
    {
        if (controlState != PlayerControlState.Gameplay)
        {
            // Do not allow actions in states other than "Gameplay"
            return;
        }
        if (true || context.performed) //TODO: remove true
        {
            StartVelocityUpdate(new Vector2(-1f * sideJumpForce, upJumpForce * 0.5f));
            if (!theSR.flipX)
            {
                theSR.flipX = true;
            }
        }
    }

    private void JumpRight(InputAction.CallbackContext context)
    {
        Debug.Log("JumpRight");
        if (controlState != PlayerControlState.Gameplay)
        {
            // Do not allow actions in states other than "Gameplay"
            return;
        }
        if (true || context.performed) //TODO: remove true
        {
            StartVelocityUpdate(new Vector2(1f * sideJumpForce, upJumpForce * 0.5f));
            if (theSR.flipX)
            {
                theSR.flipX = false;
            }
        }
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }
        else
        {
            theAnim.Play(newState);
            currentState = newState;
        }
    }

    private bool IsAnimationPlayingAndUpdateCurrentState(string stateName)
    {
        if (
            theAnim.GetCurrentAnimatorStateInfo(0).IsName(stateName)
            && theAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f
        )
        {
            currentState = stateName;
            return true;
        }
        else
        {
            return false;
        }
    }
}
