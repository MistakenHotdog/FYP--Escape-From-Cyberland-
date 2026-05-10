using UnityEngine;

public class ServerPCInteraction : MonoBehaviour
{
    public GameObject useButton;
    public ServerRoomPC serverPC;

    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (completed) return;

        if (useButton != null)
            useButton.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (useButton != null)
            useButton.SetActive(false);
    }

    public void UsePC()
    {
        if (completed) return;

        if (useButton != null)
            useButton.SetActive(false);

        if (serverPC != null)
            serverPC.OpenPC();
    }

    public void MarkCompleted()
    {
        completed = true;

        if (useButton != null)
            useButton.SetActive(false);

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;
    }
}