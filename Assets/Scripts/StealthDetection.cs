using UnityEngine;
using UnityEngine.UI;

public class StealthDetection : MonoBehaviour
{
    [Header("UI")]
    public Image stealthMeter; // Stealth bar image

    [Header("Enemies")]
    public Transform[] drones;
    public Transform[] cameras;
    public Transform[] phishingBots;

    [Header("Detection Settings")]
    public float detectionDistance = 10f;  // Distance threshold for detection
    public bool useLineOfSight = true;     // Check if player is behind obstacles
    public LayerMask obstructionMask;      // Layer mask for walls or obstacles

    private bool isDetected;

    void Update()
    {
        isDetected = CheckDetection();

        // Update UI color
        if (stealthMeter != null)
        {
            stealthMeter.color = isDetected ? Color.red : Color.green;
        }
    }

    bool CheckDetection()
    {
        Vector3 playerPosition = transform.position;

        // 1. Check drones
        foreach (Transform drone in drones)
        {
            if (drone == null) continue;

            if (Vector3.Distance(playerPosition, drone.position) <= detectionDistance)
            {
                if (!useLineOfSight || HasLineOfSight(drone.position, playerPosition))
                    return true;
            }
        }

        // 2. Check phishing bots / virus bots
        foreach (Transform bot in phishingBots)
        {
            if (bot == null) continue;

            if (Vector3.Distance(playerPosition, bot.position) <= detectionDistance)
            {
                if (!useLineOfSight || HasLineOfSight(bot.position, playerPosition))
                    return true;
            }
        }

        // 3. Check cameras (360° top-down view)
        foreach (Transform cam in cameras)
        {
            if (cam == null) continue;

            float distanceToPlayer = Vector3.Distance(playerPosition, cam.position);
            if (distanceToPlayer <= detectionDistance)
            {
                if (!useLineOfSight || HasLineOfSight(cam.position, playerPosition))
                    return true;
            }
        }

        return false; // safe
    }

    // Optional: raycast to check line-of-sight
    bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        if (Physics.Raycast(from, dir.normalized, out RaycastHit hit, dir.magnitude, obstructionMask))
        {
            // Hit an obstacle, player not visible
            return false;
        }
        return true; // Player is visible
    }

    public bool IsDetected()
    {
        return isDetected;
    }
}
