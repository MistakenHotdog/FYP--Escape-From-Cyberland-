using UnityEngine;

public class SteamBurst : MonoBehaviour
{
    public ParticleSystem steamFX;

    void Start()
    {
        if (steamFX != null)
        {
            InvokeRepeating("Burst", 1f, 4f);
        }
    }

    void Burst()
    {
        steamFX.Play();
        Invoke("StopSteam", 2f);
    }

    void StopSteam()
    {
        steamFX.Stop();
    }
}
