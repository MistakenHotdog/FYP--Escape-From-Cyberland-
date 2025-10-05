using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1.5f;      // how high to jump
    public float gravity = -9.81f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching = false;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small force to keep grounded

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Choose speed
        float speed = isCrouching ? crouchSpeed :
            (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Toggle crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? 1.0f : 2.0f;
        }
    }
}
