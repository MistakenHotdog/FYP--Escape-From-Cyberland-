using UnityEngine;

public class BugPatrol3D : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 2f;
    public float rotateSpeed = 5f;

    private int currentPoint = 0;
    private float fixedY;
    private Animator anim;

    void Start()
    {
        fixedY = transform.position.y;
        anim = GetComponent<Animator>();
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

        float distance = Vector3.Distance(transform.position, targetPos);

        // If moving → play walk animation
        if (distance > 0.3f)
            anim.SetBool("isMoving", true);
        else
            anim.SetBool("isMoving", false);

        // Move bug
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Rotate
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        // Switch waypoint
        if (distance < 0.3f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }
}
