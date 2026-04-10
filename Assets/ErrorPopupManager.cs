using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ErrorPopupManager : MonoBehaviour
{
    [Header("Popup Settings")]
    public GameObject popupPrefab;      // Your PopupPanel prefab
    public int popupCount = 6;          // Amount to spawn
    public Canvas popupCanvas;          // Assign your UI Canvas

    private int closedPopups = 0;
    private bool gamePaused = false;
    private List<GameObject> activePopups = new List<GameObject>();

    public void TriggerBugPopup()
    {
        if (gamePaused) return;

        Time.timeScale = 0f;
        gamePaused = true;

        closedPopups = 0;
        activePopups.Clear();

        SpawnPopups();
    }

    void SpawnPopups()
    {
        if (popupCanvas == null || popupPrefab == null)
        {
            Debug.LogWarning("[ErrorPopupManager] Missing popupCanvas or popupPrefab.");
            return;
        }
        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();

        for (int i = 0; i < popupCount; i++)
        {
            GameObject popup = Instantiate(popupPrefab, popupCanvas.transform);
            popup.SetActive(true);

            RectTransform popupRect = popup.GetComponent<RectTransform>();

            // Calculate random position fully inside the canvas
            float x = Random.Range(-canvasRect.rect.width / 2 + popupRect.rect.width / 2,
                                   canvasRect.rect.width / 2 - popupRect.rect.width / 2);

            float y = Random.Range(-canvasRect.rect.height / 2 + popupRect.rect.height / 2,
                                   canvasRect.rect.height / 2 - popupRect.rect.height / 2);

            popupRect.anchoredPosition = new Vector2(x, y);

            // Setup CloseButton safely
            Button closeBtn = popup.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeBtn != null)
            {
                closeBtn.onClick.AddListener(() => ClosePopup(popup));
            }
            else
            {
                Debug.LogError("CloseButton not found in popup prefab!");
            }

            activePopups.Add(popup);
        }
    }

    void ClosePopup(GameObject popup)
    {
        Destroy(popup);
        closedPopups++;

        if (closedPopups >= popupCount)
        {
            ResumeGame();
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        gamePaused = false;
    }
}
