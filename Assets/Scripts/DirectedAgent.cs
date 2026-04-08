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
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        animator = GetComponent<Animator>();

        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }

    void OnDestroy()
    {
        inputActions?.Player.Disable();
        inputActions?.Dispose();
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
        if (rb == null) return;
        rb.linearVelocity = moveInput * moveSpeed;
    }
}