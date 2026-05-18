using UnityEngine;

public class KeyVaultInteraction : MonoBehaviour
{
    public GameObject useButton;
    public KeyVaultPC vaultPC;

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

    public void UseVault()
    {
        if (completed) return;

        if (useButton != null)
            useButton.SetActive(false);

        if (vaultPC != null)
            vaultPC.OpenVault();
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