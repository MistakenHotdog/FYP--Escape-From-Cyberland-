using UnityEngine;

public class DoorwayLaserTimer : MonoBehaviour
{
    public DoorwayLaser laser;
    public float interval = 3f;

    void Start()
    {
        InvokeRepeating("ToggleLaser", 0f, interval);
    }

    void ToggleLaser()
    {
        laser.ToggleLaser();
    }
}
