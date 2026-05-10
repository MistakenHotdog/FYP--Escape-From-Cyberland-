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

        useButton.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        useButton.SetActive(false);
    }

    public void UsePC()
    {
        if (completed) return;

        useButton.SetActive(false);

        serverPC.OpenPC();
    }

    public void MarkCompleted()
    {
        completed = true;

        useButton.SetActive(false);

        GetComponent<Collider>().enabled = false;
    }
}