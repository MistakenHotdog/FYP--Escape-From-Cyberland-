using UnityEngine;

public class BugPatrol3D : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 2f;
    public float rotateSpeed = 5f;
    private int currentPoint = 0;

    private float fixedY;

    void Start()
    {
        // Lock the starting Y position
        fixedY = transform.position.y;
    }

    void Update()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPoint];

        Vector3 targetPos = new Vector3(
            target.position.x,
            fixedY,
            target.position.z
        );

        // Movement
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Rotation
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        // Switch patrol point
        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }
}
