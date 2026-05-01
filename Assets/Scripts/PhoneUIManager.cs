using UnityEngine;

public class PhoneUIManager : MonoBehaviour
{
    public GameObject phonePanel;   // Big phone UI
    public GameObject phoneIcon;    // Small icon

    private bool isOpen = false;

    // 📱 OPEN PHONE
    public void OpenPhone()
    {
        if (isOpen) return;

        phonePanel.SetActive(true);

        // Pause game
        Time.timeScale = 0f;

        isOpen = true;
    }

    // ❌ CLOSE PHONE
    public void ClosePhone()
    {
        if (!isOpen) return;

        phonePanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        isOpen = false;
    }
}