using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;
    public Collider doorBlockCollider;

    public void OpenDoor()
    {
        if (animator != null)
            animator.SetTrigger("Open");

        if (doorBlockCollider != null)
            doorBlockCollider.enabled = false;
    }
}