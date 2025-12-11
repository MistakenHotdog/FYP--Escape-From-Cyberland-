using UnityEngine;

public class DoorwayLaser : MonoBehaviour
{
    public bool isOn = true;
    private Renderer[] laserRenderers;
    private Collider[] laserColliders;

    void Start()
    {
        // Get all Renderer components in child planes
        laserRenderers = GetComponentsInChildren<Renderer>();

        // Get all Colliders in this object and its children
        laserColliders = GetComponentsInChildren<Collider>();

        UpdateLaserState();
    }

    public void ToggleLaser()
    {
        isOn = !isOn;
        UpdateLaserState();
    }

    void UpdateLaserState()
    {
        // Enable/disable all child renderers
        foreach (Renderer r in laserRenderers)
        {
            r.enabled = isOn;
        }

        // Enable/disable all child colliders
        foreach (Collider c in laserColliders)
        {
            c.enabled = isOn;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOn) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by doorway laser!");
            // Add damage or respawn here
        }
    }
}
