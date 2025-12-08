using UnityEngine;

public class DronePatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;
    public float reachDistance = 0.3f;

    [Header("Laser Points")]
    public Transform[] laserPoints;   // replace 4 separate points with array
    public float laserLength = 7f;

    [Header("Enemy Spawn Settings")]
    public GameObject bugPrefab;     // Your flying bug enemy
    public Transform spawnPoint;     // Where bug appears on drone
    public float spawnCooldown = 5f;

    private float nextSpawnTime = 0f;

    private Transform targetPoint;
    private LineRenderer[] lasers;

    private void Start()
    {
        targetPoint = pointB;

        // Setup lasers
        lasers = new LineRenderer[laserPoints.Length];
        for (int i = 0; i < laserPoints.Length; i++)
            lasers[i] = CreateLaser(laserPoints[i]);
    }

    private void Update()
    {
        Patrol();
        KeepUpright();
        UpdateLasers();
        DetectPlayerWithLaser();
    }

    // ---------------- Laser Setup ----------------

    LineRenderer CreateLaser(Transform p)
    {
        GameObject obj = new GameObject(p.name + "_Laser");
        obj.transform.parent = p;
        obj.transform.localPosition = Vector3.zero;

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.startWidth = 0.03f;
        lr.endWidth = 0.03f;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = Color.red;

        return lr;
    }

    void UpdateLasers()
    {
        foreach (LineRenderer lr in lasers)
        {
            lr.SetPosition(0, lr.transform.position);
            lr.SetPosition(1, lr.transform.position + Vector3.down * laserLength);
        }
    }

    // ---------------- Laser Detection ----------------

    void DetectPlayerWithLaser()
    {
        foreach (Transform p in laserPoints)
        {
            RaycastHit hit;

            if (Physics.Raycast(p.position, Vector3.down, out hit, laserLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    TrySpawnBug();
                }
            }
        }
    }

    void TrySpawnBug()
    {
        if (Time.time < nextSpawnTime) return;

        Instantiate(bugPrefab, spawnPoint.position, Quaternion.identity);

        nextSpawnTime = Time.time + spawnCooldown; // cooldown
    }

    // ---------------- Movement ----------------

    void Patrol()
    {
        Vector3 target = new Vector3(targetPoint.position.x, transform.position.y, targetPoint.position.z);

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target) < reachDistance)
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
    }

    void KeepUpright()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
