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
    public float speedMultiplier = 1f;

    [Header("References")]
    public Joystick joystick;
    public Animator animator;

    private Rigidbody rb;
    private Transform cam;
    private int speedHash;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speedHash = Animator.StringToHash("Speed");

        if (Camera.main != null)
            cam = Camera.main.transform;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Start()
    {
        RefreshControlMode(); // 🔥 IMPORTANT
    }

    // 🔥 NEW METHOD
    public void RefreshControlMode()
    {
        int uiType = PlayerPrefs.GetInt("UIType", 1);

        if (uiType == 1)
            controlMode = ControlMode.Joystick;
        else
            controlMode = ControlMode.Buttons;

        Debug.Log("[PlayerMove] Refreshed Control Mode: " + controlMode);
    }

    void FixedUpdate()
    {
        if (cam == null || animator == null) return;

        float h = 0f;
        float v = 0f;

        // ---------------- INPUT ----------------

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

        // ---------------- VOICE ----------------

        VoiceMotor.Tick();

        if (IsVoiceModeActive() && VoiceMotor.HasInput)
        {
            h = VoiceMotor.Horizontal;
            v = VoiceMotor.Vertical;
        }
        else if (!IsVoiceModeActive())
        {
            VoiceMotor.Stop();
        }

        // ---------------- MOVEMENT ----------------

        Vector2 input = new Vector2(h, v);
        float magnitude = Mathf.Clamp01(input.magnitude);

        if (magnitude < 0.1f)
        {
            animator.SetFloat(speedHash, 0f);
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        float baseSpeed = (magnitude >= runThreshold) ? runSpeed : walkSpeed;
        float finalSpeed = baseSpeed * speedMultiplier;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * v + right * h).normalized;

        rb.velocity = new Vector3(
            moveDir.x * finalSpeed,
            rb.velocity.y,
            moveDir.z * finalSpeed
        );

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        animator.SetFloat(speedHash, 1f);
    }

    bool IsVoiceModeActive()
    {
        return PlayerPrefs.GetInt("UIType", 1) == 3;
    }
}