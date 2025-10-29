using UnityEngine;
using UnityEngine.UI;

public class StealthDetection : MonoBehaviour
{
    public Image stealthMeter;      // Reference to UI bar or icon
    public Light[] sceneLights;     // Lights in your scene
    public float visibleThreshold = 0.4f; // Sensitivity to brightness
    private bool isVisible;

    void Update()
    {
        float exposure = GetLightExposure();
        isVisible = exposure > visibleThreshold;

        // Update the UI color
        if (stealthMeter != null)
        {
            stealthMeter.color = isVisible ? Color.red : Color.green;
        }
    }

    float GetLightExposure()
    {
        float totalLight = 0f;
        foreach (Light light in sceneLights)
        {
            if (light == null) continue;

            Vector3 dir = light.transform.position - transform.position;
            float distance = dir.magnitude;

            // Raycast to see if player is in light
            if (!Physics.Raycast(transform.position, dir, distance))
            {
                float intensity = light.intensity / Mathf.Max(1, distance);
                totalLight += intensity;
            }
        }
        return totalLight;
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}
