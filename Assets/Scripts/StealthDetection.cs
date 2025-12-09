using UnityEngine;
using UnityEngine.UI;

public class StealthDetection : MonoBehaviour
{
    [Header("UI")]
    public Image stealthMeter;

    [Header("Glow Feedback")]
    public StealthGlowController glowController;   // <--- ADDED

    [Header("Player Health Reference")]
    public PlayerHealth playerHealth;
    public float detectionDamage = 5f;

    [Header("Enemies")]
    public Transform[] drones;
    public Transform[] phishingBots;
    public SurveillanceCamera[] cameras;

    [Header("Detection Settings")]
    public float detectionDistance = 12f;
    public bool useLineOfSight = true;
    public LayerMask obstructionMask;

    private bool isDetected = false;

    void Update()
    {
        isDetected = CheckDetection();

        // Change UI color
        if (stealthMeter != null)
            stealthMeter.color = isDetected ? Color.red : Color.green;

        // Change glow color under player
        if (glowController != null)
            glowController.SetDetected(isDetected);   // <-- ADDED

        // Damage over time when detected
        if (isDetected && playerHealth != null)
            playerHealth.TakeDamage(detectionDamage * Time.deltaTime);
    }

    bool CheckDetection()
    {
        Vector3 playerPos = transform.position;

        // 1. Drones
        foreach (Transform drone in drones)
        {
            if (drone == null) continue;

            if (Vector3.Distance(playerPos, drone.position) <= detectionDistance)
            {
                if (!useLineOfSight || HasLineOfSight(drone.position, playerPos))
                    return true;
            }
        }

        // 2. Bots
        foreach (Transform bot in phishingBots)
        {
            if (bot == null) continue;

            if (Vector3.Distance(playerPos, bot.position) <= detectionDistance)
            {
                if (!useLineOfSight || HasLineOfSight(bot.position, playerPos))
                    return true;
            }
        }

        // 3. Cameras
        foreach (SurveillanceCamera cam in cameras)
        {
            if (cam == null) continue;

            if (cam.IsDetectingPlayer)
                return true;

            if (Vector3.Distance(playerPos, cam.transform.position) <= detectionDistance)
            {
                if (!useLineOfSight || HasLineOfSight(cam.transform.position, playerPos))
                    return true;
            }
        }

        return false;
    }

    bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;

        if (Physics.Raycast(from, dir.normalized, out RaycastHit hit, dir.magnitude, obstructionMask))
            return false;

        return true;
    }

    public bool IsDetected() => isDetected;
}
