using UnityEngine;
using System.Collections;

public class SurveillanceCamera : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Points the camera will look at in sequence")]
    public Transform[] lookPoints;

    [Tooltip("How long camera stays focused on each point")]
    public float focusTime = 2f;

    [Tooltip("Speed of camera rotation")]
    public float rotationSpeed = 30f;

    [Tooltip("If true, camera rotates back and forth. If false, loops through all points")]
    public bool pingPongMode = true;

    [Header("Laser Settings")]
    [Tooltip("Where the laser originates from (child object of camera)")]
    public Transform laserOrigin;

    [Tooltip("Maximum distance the laser can reach")]
    public float laserMaxDistance = 50f;

    [Tooltip("Color of the laser line")]
    public Color laserColor = Color.red;

    [Tooltip("Width of the laser line")]
    public float laserWidth = 0.05f;

    [Tooltip("Should laser be visible")]
    public bool showLaser = true;

    [Header("Detection Settings")]
    [Tooltip("Layers the laser can detect (usually Player layer)")]
    public LayerMask detectionLayers;

    [Tooltip("Time before camera resets after losing sight of player")]
    public float alertCooldown = 3f;

    [Tooltip("Event triggered when player is detected")]
    public UnityEngine.Events.UnityEvent onPlayerDetected;

    [Tooltip("Event triggered when player is lost")]
    public UnityEngine.Events.UnityEvent onPlayerLost;

    [Header("Audio")]
    public AudioClip detectionSound;
    public AudioClip alertSound;
    private AudioSource audioSource;

    [Header("Visual Feedback")]
    public Light cameraLight;
    public Color normalLightColor = Color.white;
    public Color alertLightColor = Color.red;
    public float alertBlinkSpeed = 0.5f;

    // Private variables
    private LineRenderer laserLine;
    private int currentPointIndex = 0;
    private bool isRotating = false;
    private bool isPlayerDetected = false;
    private bool isInAlertMode = false;
    private Quaternion targetRotation;
    private float focusTimer = 0f;
    private Coroutine alertCoroutine;
    private int rotationDirection = 1; // 1 for forward, -1 for backward

    private void Awake()
    {
        SetupLaser();
        SetupAudio();

        // Create laser origin if not assigned
        if (laserOrigin == null)
        {
            GameObject origin = new GameObject("LaserOrigin");
            origin.transform.SetParent(transform);
            origin.transform.localPosition = Vector3.zero;
            origin.transform.localRotation = Quaternion.identity;
            laserOrigin = origin.transform;
        }
    }

    private void Start()
    {
        if (lookPoints == null || lookPoints.Length == 0)
        {
            Debug.LogWarning("SurveillanceCamera: No look points assigned! Camera will not rotate.");
            return;
        }

        // Start looking at first point
        if (lookPoints[0] != null)
        {
            targetRotation = Quaternion.LookRotation(lookPoints[0].position - transform.position);
        }

        // Set initial light color
        if (cameraLight != null)
        {
            cameraLight.color = normalLightColor;
        }
    }

    private void Update()
    {
        if (!isInAlertMode)
        {
            RotateBetweenPoints();
        }

        DrawAndCheckLaser();
    }

    private void SetupLaser()
    {
        // Create LineRenderer for laser
        laserLine = gameObject.GetComponent<LineRenderer>();
        if (laserLine == null)
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
        }

        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth;
        laserLine.material = new Material(Shader.Find("Sprites/Default"));
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;
        laserLine.positionCount = 2;
        laserLine.enabled = showLaser;

        // Make laser render on top
        laserLine.sortingOrder = 1000;
    }

    private void SetupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    private void RotateBetweenPoints()
    {
        if (lookPoints == null || lookPoints.Length == 0) return;

        // Rotate towards target
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Check if reached target rotation
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            focusTimer += Time.deltaTime;

            // Wait at current point
            if (focusTimer >= focusTime)
            {
                focusTimer = 0f;
                SetNextLookPoint();
            }
        }
    }

    private void SetNextLookPoint()
    {
        if (pingPongMode)
        {
            // Ping pong between points
            currentPointIndex += rotationDirection;

            // Reverse direction at ends
            if (currentPointIndex >= lookPoints.Length)
            {
                currentPointIndex = lookPoints.Length - 2;
                rotationDirection = -1;
            }
            else if (currentPointIndex < 0)
            {
                currentPointIndex = 1;
                rotationDirection = 1;
            }
        }
        else
        {
            // Loop through points
            currentPointIndex = (currentPointIndex + 1) % lookPoints.Length;
        }

        // Set new target rotation
        if (lookPoints[currentPointIndex] != null)
        {
            targetRotation = Quaternion.LookRotation(
                lookPoints[currentPointIndex].position - transform.position
            );
        }
    }

    private void DrawAndCheckLaser()
    {
        if (laserOrigin == null) return;

        Vector3 startPos = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;

        laserLine.SetPosition(0, startPos);

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(startPos, direction, out hit, laserMaxDistance, detectionLayers);

        if (hitSomething)
        {
            // Laser hit something
            laserLine.SetPosition(1, hit.point);

            // Check if it's the player
            if (hit.collider.CompareTag("Player"))
            {
                if (!isPlayerDetected)
                {
                    OnPlayerEnterDetection();
                }
                isPlayerDetected = true;
            }
            else
            {
                if (isPlayerDetected)
                {
                    OnPlayerExitDetection();
                }
                isPlayerDetected = false;
            }
        }
        else
        {
            // Laser didn't hit anything
            laserLine.SetPosition(1, startPos + direction * laserMaxDistance);

            if (isPlayerDetected)
            {
                OnPlayerExitDetection();
            }
            isPlayerDetected = false;
        }

        // Debug ray in Scene view
        Debug.DrawRay(startPos, direction * laserMaxDistance, laserColor);
    }

    private void OnPlayerEnterDetection()
    {
        Debug.Log("SurveillanceCamera: Player detected!");

        // Trigger alert mode
        isInAlertMode = true;

        // Play detection sound
        if (audioSource != null && detectionSound != null)
        {
            audioSource.PlayOneShot(detectionSound);
        }

        // Start alert visual
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
        }
        alertCoroutine = StartCoroutine(AlertMode());

        // Invoke event
        onPlayerDetected?.Invoke();
    }

    private void OnPlayerExitDetection()
    {
        Debug.Log("SurveillanceCamera: Player lost!");

        // Invoke event
        onPlayerLost?.Invoke();

        // Return to normal after cooldown
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
        }
        alertCoroutine = StartCoroutine(ReturnToNormal());
    }

    private IEnumerator AlertMode()
    {
        // Play alert sound
        if (audioSource != null && alertSound != null)
        {
            audioSource.PlayOneShot(alertSound);
        }

        // Blink light while player is detected
        while (isPlayerDetected)
        {
            if (cameraLight != null)
            {
                cameraLight.color = alertLightColor;
                yield return new WaitForSeconds(alertBlinkSpeed);
                cameraLight.color = normalLightColor;
                yield return new WaitForSeconds(alertBlinkSpeed);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator ReturnToNormal()
    {
        yield return new WaitForSeconds(alertCooldown);

        isInAlertMode = false;

        if (cameraLight != null)
        {
            cameraLight.color = normalLightColor;
        }

        Debug.Log("SurveillanceCamera: Returning to normal patrol");
    }

    // Public methods for external control
    public void SetAlertMode(bool alert)
    {
        isInAlertMode = alert;
    }

    public bool IsPlayerCurrentlyDetected()
    {
        return isPlayerDetected;
    }

    public void DisableCamera()
    {
        enabled = false;
        if (laserLine != null)
            laserLine.enabled = false;
        if (cameraLight != null)
            cameraLight.enabled = false;
    }

    public void EnableCamera()
    {
        enabled = true;
        if (laserLine != null)
            laserLine.enabled = showLaser;
        if (cameraLight != null)
            cameraLight.enabled = true;
    }

    // ✅ Added for StealthDetection compatibility
    public bool IsDetectingPlayer => isPlayerDetected;

    // Visualization in editor
    private void OnDrawGizmos()
    {
        if (lookPoints == null || lookPoints.Length == 0) return;

        Gizmos.color = Color.yellow;

        // Draw lines to look points
        foreach (Transform point in lookPoints)
        {
            if (point != null)
            {
                Gizmos.DrawLine(transform.position, point.position);
                Gizmos.DrawWireSphere(point.position, 0.3f);
            }
        }

        // Draw laser direction
        if (laserOrigin != null)
        {
            Gizmos.color = laserColor;
            Gizmos.DrawRay(laserOrigin.position, laserOrigin.forward * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (laserOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(laserOrigin.position, laserOrigin.forward * laserMaxDistance);
        }
    }
}
