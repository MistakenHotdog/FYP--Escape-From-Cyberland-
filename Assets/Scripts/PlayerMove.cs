using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public enum ControlMode { Joystick, Buttons }

    [Header("Control Mode")]
    public ControlMode controlMode = ControlMode.Joystick;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;
    public float runThreshold = 0.9f;

    [Header("Effects")]
    [Range(0f, 1f)]
    public float speedMultiplier = 1f; // 🔥 FIXED (for your error)

    [Header("References")]
    public Joystick joystick;
    public Animator animator;

    private Rigidbody rb;
    private Transform cam;
    private int speedHash;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        speedHash = Animator.StringToHash("Speed");

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        ApplyMode(PlayerPrefs.GetInt("ControlMode", 0));
    }

    void OnEnable()
    {
        ControlSettings.OnControlChanged += ApplyMode;
    }

    void OnDisable()
    {
        ControlSettings.OnControlChanged -= ApplyMode;
    }

    void ApplyMode(int mode)
    {
        controlMode = (mode == 0) ? ControlMode.Joystick : ControlMode.Buttons;
    }

    void FixedUpdate()
    {
        float h = 0f;
        float v = 0f;

        // 🎮 INPUT SYSTEM SWITCH
        if (controlMode == ControlMode.Joystick)
        {
            if (joystick != null)
            {
                h = joystick.Horizontal;
                v = joystick.Vertical;
            }
        }
        else
        {
            h = ButtonInput.Horizontal;
            v = ButtonInput.Vertical;
        }

        Vector2 input = new Vector2(h, v);
        float magnitude = Mathf.Clamp01(input.magnitude);

        // 🚫 Stop movement if no input
        if (magnitude < 0.1f)
        {
            animator.SetFloat(speedHash, 0f);
            animator.speed = 1f;
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        // 🏃 Speed calculation (WITH multiplier)
        float baseSpeed = (magnitude >= runThreshold) ? runSpeed : walkSpeed;
        float finalSpeed = baseSpeed * speedMultiplier;

        // 🎥 Camera-relative movement
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * v + right * h).normalized * finalSpeed;

        // 🚀 Apply movement
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        // 🔄 Smooth rotation
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        // 🎭 Animation
        animator.SetFloat(speedHash, 1f);
        animator.speed = finalSpeed / walkSpeed;
    }
}