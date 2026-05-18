using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class StealthGlowController : MonoBehaviour
{
    [Header("Materials")]
    public Material safeMaterial;
    public Material detectedMaterial;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        SetSafe(); // default at start
    }

    public void SetDetected(bool detected)
    {
        rend.material = detected ? detectedMaterial : safeMaterial;
    }

    // Useful if you want to call manually through inspector
    [ContextMenu("Set Safe")]
    public void SetSafe() => rend.material = safeMaterial;

    [ContextMenu("Set Detected")]
    public void SetDetectedColor() => rend.material = detectedMaterial;
}
