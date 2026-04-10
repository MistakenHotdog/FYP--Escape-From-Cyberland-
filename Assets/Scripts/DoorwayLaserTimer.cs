using UnityEngine;
using System.Collections;

public class DoorwayLaserTimer : MonoBehaviour
{
    public DoorwayLaser laser;

    public float interval = 3f;
    public float randomVariation = 0f;

    void Start()
    {
        StartCoroutine(LaserCycle());
    }

    IEnumerator LaserCycle()
    {
        while (true)
        {
            float wait = interval + Random.Range(-randomVariation, randomVariation);
            yield return new WaitForSeconds(Mathf.Max(0.5f, wait));
            if (laser != null)
                laser.ToggleLaser();
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
