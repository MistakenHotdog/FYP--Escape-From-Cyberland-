using UnityEngine;

public class SteamBurst : MonoBehaviour
{
    public ParticleSystem steamFX;
    public ParticleSystem steamFX2;

    void Start()
    {
        if (steamFX != null || steamFX2 != null)
        {
            InvokeRepeating("Burst", 1f, 4f);
        }
    }

    void Burst()
    {
        if (steamFX != null) steamFX.Play();
        if (steamFX2 != null) steamFX2.Play();
        Invoke("StopSteam", 2f);
    }

    void StopSteam()
    {
        if (steamFX != null) steamFX.Stop();
        if (steamFX2 != null) steamFX2.Stop();
    }

    void OnDestroy()
    {
        CancelInvoke();
    }
}
