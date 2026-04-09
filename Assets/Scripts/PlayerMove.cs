using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public enum ControlMode
    {
        Joystick,
        Buttons
    }

    [Header("Control Mode")]
    public ControlMode controlMode = ControlMode.Joystick;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float runThreshold = 0.9f;

    [Header("References")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator animator;

    [Header("Temporary Effects")]
    [Range(0f, 1f)]
    public float speedMultiplier = 1f;

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

    void Start()
    {
        int savedMode = PlayerPrefs.GetInt("ControlMode", 0);

        if (savedMode == 0)
            controlMode = ControlMode.Joystick;
        else
            controlMode = ControlMode.Buttons;
    }

    void FixedUpdate()
    {
        float h = 0f;
        float v = 0f;
        Debug.Log("H: " + h + " V: " + v);

        // 🔥 SWITCH INPUT SYSTEM
        if (controlMode == ControlMode.Joystick)
        {
            h = joystick.Horizontal;
            v = joystick.Vertical;
        }
        else if (controlMode == ControlMode.Buttons)
        {
            h = ButtonInput.Horizontal;
            v = ButtonInput.Vertical;
        }

        Vector2 input = new Vector2(h, v);
        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        float currentSpeed = ((inputMagnitude >= runThreshold) ? runSpeed : walkSpeed) * speedMultiplier;

        if (inputMagnitude < 0.1f)
        {
            animator.SetFloat(speedHash, 0f);
            animator.speed = 1f;
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

        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        animator.SetFloat(speedHash, 1f);
        animator.speed = currentSpeed / walkSpeed;
    }
    void OnEnable()
    {
        ControlSettings.ControlChanged += UpdateControlMode;
    }

    void OnDisable()
    {
        ControlSettings.ControlChanged -= UpdateControlMode;
    }

    void UpdateControlMode(int mode)
    {
        controlMode = (mode == 0) ? ControlMode.Joystick : ControlMode.Buttons;
    }
}