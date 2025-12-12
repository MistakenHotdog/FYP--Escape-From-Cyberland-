using UnityEngine;

public class SteamBurst : MonoBehaviour
{
    public ParticleSystem steamFX;
    public ParticleSystem steamFX2;

    void Start()
    {
        if (steamFX  != null)
        {
            InvokeRepeating("Burst", 1f, 4f);
        }
    }

    void Burst()
    {
        steamFX.Play();
        steamFX2.Play();
        Invoke("StopSteam", 2f);
    }

    void StopSteam()
    {
        steamFX.Stop();
        steamFX2.Stop();
    }
}
