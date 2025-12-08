using UnityEngine;

public class GlowPulse3D : MonoBehaviour
{
    public float pulseSpeed = 2f;     // How fast it pulses
    public float pulseStrength = 0.3f; // How much it grows/shrinks

    private Vector3 startScale;

    void Start()
    {
        // Store the size you set manually in Inspector
        startScale = transform.localScale;
    }

    void Update()
    {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float scale = 1f + pulse * pulseStrength;

        transform.localScale = new Vector3(
            startScale.x * scale,
            startScale.y,
            startScale.z * scale
        );
    }
}
