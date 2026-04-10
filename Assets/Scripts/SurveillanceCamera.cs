using UnityEngine;
using System.Collections;

public class SurveillanceCamera : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform[] lookPoints;
    public float focusTime = 2f;
    public float rotationSpeed = 30f;
    public bool pingPongMode = true;

    [Header("Laser Settings")]
    public Transform laserOrigin;
    public float laserMaxDistance = 50f;
    public Color laserColor = Color.red;
    public float laserWidth = 0.05f;
    public bool showLaser = true;

    [Header("Detection Settings")]
    public LayerMask detectionLayers;
    public float alertCooldown = 3f;

    public UnityEngine.Events.UnityEvent onPlayerDetected;
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

    private LineRenderer laserLine;
    private int currentPointIndex = 0;
    private bool isPlayerDetected = false;
    private bool isInAlertMode = false;
    private Quaternion targetRotation;
    private float focusTimer = 0f;
    private Coroutine alertCoroutine;
    private int rotationDirection = 1;
    private AlarmSystem cachedAlarm;

    private static Shader _cachedLaserShader;
    private static Shader CameraLaserShader
    {
        get
        {
            if (_cachedLaserShader == null)
                _cachedLaserShader = Shader.Find("Sprites/Default");
            return _cachedLaserShader;
        }
    }

    private void Awake()
    {
        SetupLaser();
        SetupAudio();
        cachedAlarm = FindObjectOfType<AlarmSystem>();

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
            Debug.LogWarning("No look points assigned!");
            return;
        }

        if (lookPoints[0] != null)
        {
            targetRotation = Quaternion.LookRotation(lookPoints[0].position - transform.position);
        }

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

    void SetupLaser()
    {
        laserLine = GetComponent<LineRenderer>();
        if (laserLine == null)
            laserLine = gameObject.AddComponent<LineRenderer>();

        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth;
        Shader shader = CameraLaserShader;
        laserLine.material = shader != null ? new Material(shader) : new Material(Shader.Find("Unlit/Color"));
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;
        laserLine.positionCount = 2;
        laserLine.enabled = showLaser;
        laserLine.sortingOrder = 1000;
    }

    void SetupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void RotateBetweenPoints()
    {
        if (lookPoints.Length == 0) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            focusTimer += Time.deltaTime;

            if (focusTimer >= focusTime)
            {
                focusTimer = 0f;
                SetNextLookPoint();
            }
        }
    }

    void SetNextLookPoint()
    {
        if (pingPongMode)
        {
            currentPointIndex += rotationDirection;

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
            currentPointIndex = (currentPointIndex + 1) % lookPoints.Length;
        }

        if (lookPoints[currentPointIndex] != null)
        {
            targetRotation = Quaternion.LookRotation(
                lookPoints[currentPointIndex].position - transform.position
            );
        }
    }

    void DrawAndCheckLaser()
    {
        if (laserOrigin == null) return;

        Vector3 startPos = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;

        laserLine.SetPosition(0, startPos);

        if (Physics.Raycast(startPos, direction, out RaycastHit hit, laserMaxDistance, detectionLayers))
        {
            laserLine.SetPosition(1, hit.point);

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
            laserLine.SetPosition(1, startPos + direction * laserMaxDistance);

            if (isPlayerDetected)
            {
                OnPlayerExitDetection();
            }
            isPlayerDetected = false;
        }
    }

    // 🔥 UPDATED HERE
    void OnPlayerEnterDetection()
    {
        Debug.Log("Player detected!");

        isInAlertMode = true;

        if (cachedAlarm != null)
            cachedAlarm.TriggerAlarm();

        if (audioSource && detectionSound)
            audioSource.PlayOneShot(detectionSound);

        if (alertCoroutine != null)
            StopCoroutine(alertCoroutine);

        alertCoroutine = StartCoroutine(AlertMode());

        onPlayerDetected?.Invoke();
    }

    void OnPlayerExitDetection()
    {
        Debug.Log("Player lost!");

        onPlayerLost?.Invoke();

        if (alertCoroutine != null)
            StopCoroutine(alertCoroutine);

        alertCoroutine = StartCoroutine(ReturnToNormal());
    }

    IEnumerator AlertMode()
    {
        if (audioSource && alertSound)
            audioSource.PlayOneShot(alertSound);

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

    IEnumerator ReturnToNormal()
    {
        yield return new WaitForSeconds(alertCooldown);

        isInAlertMode = false;

        if (cameraLight != null)
            cameraLight.color = normalLightColor;
    }

    public bool IsDetectingPlayer => isPlayerDetected;

    public void DisableCamera()
    {
        enabled = false;

        if (cameraLight != null)
            cameraLight.enabled = false;

        if (laserOrigin != null)
            laserOrigin.gameObject.SetActive(false);

        Debug.Log("Camera disabled!");
    }
}