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
    public Transform[] laserPoints;
    public float laserLength = 7f;

    [Header("Enemy Spawn Settings")]
    public GameObject bugPrefab;
    public Transform spawnPoint;
    public float spawnCooldown = 5f;

    private float nextSpawnTime = 0f;

    private Transform targetPoint;
    private LineRenderer[] lasers;

    private static Shader _cachedLaserShader;
    private static Shader LaserShader
    {
        get
        {
            if (_cachedLaserShader == null)
                _cachedLaserShader = Shader.Find("Unlit/Color");
            return _cachedLaserShader;
        }
    }

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning($"[DronePatrol] {name}: Missing patrol points.");
            enabled = false;
            return;
        }

        targetPoint = pointB;

        if (laserPoints == null) laserPoints = new Transform[0];

        lasers = new LineRenderer[laserPoints.Length];
        for (int i = 0; i < laserPoints.Length; i++)
        {
            if (laserPoints[i] != null)
                lasers[i] = CreateLaser(laserPoints[i]);
        }
    }

    private void Update()
    {
        if (targetPoint == null) return;

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
        Shader shader = LaserShader;
        if (shader != null)
        {
            lr.material = new Material(shader);
            lr.material.color = Color.red;
        }

        return lr;
    }

    void UpdateLasers()
    {
        if (lasers == null) return;
        foreach (LineRenderer lr in lasers)
        {
            if (lr == null) continue;
            lr.SetPosition(0, lr.transform.position);
            lr.SetPosition(1, lr.transform.position + Vector3.down * laserLength);
        }
    }

    // ---------------- Laser Detection ----------------

    void DetectPlayerWithLaser()
    {
        if (laserPoints == null) return;
        foreach (Transform p in laserPoints)
        {
            if (p == null) continue;

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
        if (bugPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning($"[DronePatrol] {name}: Missing bugPrefab or spawnPoint.");
            return;
        }

        Instantiate(bugPrefab, spawnPoint.position, Quaternion.identity);
        nextSpawnTime = Time.time + spawnCooldown;
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
