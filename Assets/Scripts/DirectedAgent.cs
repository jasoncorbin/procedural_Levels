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
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
        else
        {
            Debug.LogWarning("DirectedAgent: no Rigidbody2D found on " + gameObject.name);
        }

        animator = GetComponent<Animator>();
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions?.Player.Enable();
    }

    void OnDisable()
    {
        inputActions?.Player.Disable();
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        Debug.Log("MOVE INPUT: " + moveInput + " | Speed: " + moveInput.magnitude);

        if (animator != null)
        {
            animator.SetFloat("velocityX", moveInput.x);
            animator.SetFloat("velocityY", moveInput.y);
            animator.SetFloat("speed", moveInput.magnitude);
        }
    }

    void FixedUpdate()
    {
        Debug.Log("FIXED UPDATE - applying velocity: " + (moveInput * moveSpeed));
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
