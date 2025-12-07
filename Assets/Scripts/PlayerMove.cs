using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

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

        // Freeze unwanted physics rotations
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Debug.Log("Joystick X: " + h + " | Y: " + v);

        Vector3 move = new Vector3(h, 0, v);
        rb.velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);

        // Early exit for performance
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            animator.SetFloat(speedHash, 0f);
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

        moveDir = (forward * v + right * h).normalized * moveSpeed;

        // Apply movement physics
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        // Smooth rotation
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Animation blend
        animator.SetFloat(speedHash, moveDir.magnitude / moveSpeed);
    }
}
