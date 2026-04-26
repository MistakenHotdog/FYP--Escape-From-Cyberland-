using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;

    public void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }
}