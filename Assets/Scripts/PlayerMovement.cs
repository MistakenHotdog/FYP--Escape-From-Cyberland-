using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;                   // Movement speed
    public float rotationSmoothness = 0.15f;   // Smooth turning

    [Header("References")]
    public Joystick joystick;                  // Mobile joystick reference

    private Animator animator;                 // Animator on child object
    private Rigidbody rb;                      // Rigidbody on parent
    private Vector3 moveDirection;             // Calculated move direction

    void Start()
    {
        // Get Rigidbody from this (parent) object
        rb = GetComponent<Rigidbody>();

        // Get Animator from child (CharacterMesh)
        animator = GetComponentInChildren<Animator>();

        // Safety check
        if (rb == null)
            Debug.LogError("Rigidbody missing! Add Rigidbody to Player parent.");
        if (animator == null)
            Debug.LogError("Animator missing! Make sure your character model is a child of Player.");
    }

    void FixedUpdate()
    {
        if (joystick == null || rb == null || animator == null) return;

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Movement logic
        if (moveDirection.magnitude >= 0.1f)
        {
            // Animate walk/run blend
            animator.SetFloat("Speed", moveDirection.magnitude);

            // Calculate movement
            Vector3 move = moveDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            // Smooth rotation
            Vector3 targetForward = Vector3.Slerp(transform.forward, moveDirection, rotationSmoothness);
            transform.forward = targetForward;
        }
        else
        {
            // Idle animation
            animator.SetFloat("Speed", 0f);
        }
    }
}
