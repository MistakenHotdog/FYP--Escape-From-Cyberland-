using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    // Smooth left-right "falling" wobble
    public IEnumerator FallingShake(float duration, float magnitude, float rotationMagnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Smooth left-right rotation wobble
            float wobble = Mathf.Sin(elapsed * 4f) * rotationMagnitude;

            // Optional slight positional shake
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);
            transform.localRotation = Quaternion.Euler(0, 0, wobble);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Restore original position and rotation
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }
}
