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
    public Transform laserFrontLeft;
    public Transform laserFrontRight;
    public Transform laserBackLeft;
    public Transform laserBackRight;
    public float laserLength = 5f;
    [Range(0, 90)]
    public float laserDownAngle = 45f; // Angle pointing down

    private Transform targetPoint;

    private LineRenderer laserFL;
    private LineRenderer laserFR;
    private LineRenderer laserBL;
    private LineRenderer laserBR;

    private void Start()
    {
        targetPoint = pointB;

        // Initialize lasers
        laserFL = CreateLaser(laserFrontLeft);
        laserFR = CreateLaser(laserFrontRight);
        laserBL = CreateLaser(laserBackLeft);
        laserBR = CreateLaser(laserBackRight);
    }

    private void Update()
    {
        Patrol();
        KeepUpright();
        UpdateLasers();
    }

    void Patrol()
    {
        if (!pointA || !pointB) return;

        Vector3 target = new Vector3(
            targetPoint.position.x,
            transform.position.y,
            targetPoint.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        Vector3 dir = target - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, target) < reachDistance)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void KeepUpright()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    LineRenderer CreateLaser(Transform laserPoint)
    {
        GameObject laserObj = new GameObject(laserPoint.name + "_Laser");
        laserObj.transform.parent = laserPoint;
        laserObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = laserObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = Color.red;
        return lr;
    }

    void UpdateLasers()
    {
        UpdateLaser(laserFL);
        UpdateLaser(laserFR);
        UpdateLaser(laserBL);
        UpdateLaser(laserBR);
    }

    void UpdateLaser(LineRenderer lr)
    {
        if (!lr) return;

        lr.SetPosition(0, lr.transform.position);

        // Point directly downward in world space
        Vector3 direction = Vector3.down;

        lr.SetPosition(1, lr.transform.position + direction * laserLength);
    }
}
