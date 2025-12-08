using UnityEngine;

public class DoorwayLaser : MonoBehaviour
{
    public bool isOn = true;
    private Renderer[] laserRenderers;
    private Collider laserCollider;

    void Start()
    {
        // Get all Renderer components in child planes
        laserRenderers = GetComponentsInChildren<Renderer>();
        laserCollider = GetComponent<Collider>();
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

        // Enable/disable the collider
        if (laserCollider != null)
            laserCollider.enabled = isOn;
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
