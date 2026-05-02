using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;
    public Collider doorBlockCollider; // 👈 assign this

    public void OpenDoor()
    {
        if (animator != null)
            animator.SetTrigger("Open");

        // 🔥 Disable blocking collider
        if (doorBlockCollider != null)
            doorBlockCollider.enabled = false;
    }
}