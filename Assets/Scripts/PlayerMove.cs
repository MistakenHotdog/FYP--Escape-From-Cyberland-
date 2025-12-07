using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float runThreshold = 0.9f;

    [Header("References")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private Transform cam;
    private int speedHash;
    private Vector3 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        speedHash = Animator.StringToHash("Speed");

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                 RigidbodyConstraints.FreezeRotationY |
                 RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Vector2 joystickInput = new Vector2(h, v);
        float inputMagnitude = Mathf.Clamp01(joystickInput.magnitude);

        // Decide current speed
        float currentSpeed = (inputMagnitude >= runThreshold) ? runSpeed : walkSpeed;

        // Stop if joystick is nearly idle
        if (inputMagnitude < 0.1f)
        {
            animator.SetFloat(speedHash, 0f);
            animator.speed = 1f; // Reset animation speed
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        // Camera-relative movement
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDir = (forward * v + right * h).normalized * currentSpeed;

        // Apply movement
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        // Smooth rotation
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Animation blend (always walking)
        animator.SetFloat(speedHash, 1f);

        // Adjust animation speed proportional to movement speed
        animator.speed = currentSpeed / walkSpeed;
        // Example: walking = 1x speed, running = 2x speed if runSpeed = 2 * walkSpeed
    }
}
