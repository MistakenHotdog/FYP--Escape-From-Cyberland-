using UnityEngine;

public class BugPatrol3D : MonoBehaviour
{
    public Transform[] patrolPoints;   // Must be exactly 2 points
    public float speed = 2f;
    public float rotateSpeed = 3f;

    private int currentPoint = 0;
    private float fixedY;

    private bool isRotating = false;
    private Quaternion targetRotation;

    void Start()
    {
        fixedY = transform.position.y;

        if (patrolPoints.Length != 2)
            Debug.LogWarning("BugPatrol3D needs exactly 2 patrol points!");
    }

    void Update()
    {
        if (patrolPoints.Length < 2) return;

        // If currently rotating 180°
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // Check if rotation is almost done
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isRotating = false; // finished rotation
            }

            return; // do not move while rotating
        }

        // Move toward current patrol point
        Transform target = patrolPoints[currentPoint];

        Vector3 targetPos = new Vector3(target.position.x, fixedY, target.position.z);
        float distance = Vector3.Distance(transform.position, targetPos);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // When bug reaches the point
        if (distance < 0.2f)
        {
            // Next point (back and forth)
            currentPoint = currentPoint == 0 ? 1 : 0;

            // Set 180° turn
            targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            isRotating = true;
        }
    }
}
