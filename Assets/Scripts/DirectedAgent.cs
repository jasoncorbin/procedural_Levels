using UnityEngine;
using UnityEngine.InputSystem;

public class DirectedAgent : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    Rigidbody2D rb;
    Animator animator;
    InputSystem_Actions inputActions;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        if (animator != null)
        {
            animator.SetFloat("velocityX", moveInput.x);
            animator.SetFloat("velocityY", moveInput.y);
            animator.SetFloat("speed", moveInput.magnitude);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.magnitude > 0.1f ? moveInput * moveSpeed : Vector2.zero;
    }
    
}
