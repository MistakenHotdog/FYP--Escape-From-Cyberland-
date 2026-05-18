using UnityEngine;
using UnityEngine.EventSystems;

public class TopDownCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 6f;
    public float height = 8f;

    public float rotationSpeed = 0.2f;
    public float smoothSpeed = 8f;

    // LIMITS (important for top-down)
    public float minPitch = 30f;
    public float maxPitch = 60f;

    private float yaw = 0f;
    private float pitch = 45f; // default angle like your image

    void Update()
    {
        HandleTouch();
        HandleTouchEditor();
    }

    void LateUpdate()
    {
        if (!target) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + offset + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target.position);
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (touch.phase == TouchPhase.Moved)
        {
            // Horizontal (rotate around player)
            yaw += touch.deltaPosition.x * rotationSpeed;

            // Vertical (slight tilt only)
            pitch -= touch.deltaPosition.y * rotationSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }
    void HandleTouchEditor()
    {
        // MOBILE TOUCH
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Ignore left side (joystick area)
            if (touch.position.x < Screen.width / 2)
                return;

            if (touch.phase == TouchPhase.Moved)
            {
                yaw += touch.deltaPosition.x * rotationSpeed;
                pitch -= touch.deltaPosition.y * rotationSpeed;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
        }

        // EDITOR (Mouse)
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            yaw += Input.GetAxis("Mouse X") * 5f;
            pitch -= Input.GetAxis("Mouse Y") * 5f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
#endif
    }
}