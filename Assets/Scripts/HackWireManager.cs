using System.Collections.Generic;
using UnityEngine;

public class HackWireManager : MonoBehaviour
{
    public GameObject hackPanel;
    public GameObject linePrefab;

    private HackNodeUI startNode;
    private GameObject currentLine;

    public List<Vector2Int> correctConnections = new List<Vector2Int>()
    {
        new Vector2Int(0,0),
        new Vector2Int(1,1),
        new Vector2Int(2,2)
    };

    private List<Vector2Int> playerConnections = new List<Vector2Int>();

    public int maxAttempts = 2;
    private int attempts = 0;

    // Player reference (disable movement instead of pausing time)
    private MonoBehaviour playerController;

    void Start()
    {
        // Find your player movement script here
        playerController = FindObjectOfType<PlayerMove>();
    }

    void Update()
    {
        if (currentLine != null && startNode != null)
        {
            Vector3 startScreen = RectTransformUtility.WorldToScreenPoint(null, startNode.transform.position);
            UpdateLine(startScreen, Input.mousePosition);
        }
    }

    public void StartConnection(HackNodeUI node)
    {
        startNode = node;
        currentLine = Instantiate(linePrefab, hackPanel.transform);
    }

    public void EndConnection(HackNodeUI endNode)
    {
        if (startNode == null || currentLine == null) return;

        Vector3 startScreen = RectTransformUtility.WorldToScreenPoint(null, startNode.transform.position);
        Vector3 endScreen = RectTransformUtility.WorldToScreenPoint(null, endNode.transform.position);

        UpdateLine(startScreen, endScreen);

        playerConnections.Add(new Vector2Int(startNode.nodeID, endNode.nodeID));

        startNode = null;
        currentLine = null;

        CheckConnections();
    }

    void UpdateLine(Vector3 startScreen, Vector3 endScreen)
    {
        RectTransform canvasRect = hackPanel.GetComponent<RectTransform>();
        RectTransform lineRect = currentLine.GetComponent<RectTransform>();

        Vector2 startLocal;
        Vector2 endLocal;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startScreen, null, out startLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, endScreen, null, out endLocal);

        Vector2 direction = endLocal - startLocal;
        float distance = direction.magnitude;

        lineRect.anchoredPosition = startLocal + direction / 2f;
        lineRect.sizeDelta = new Vector2(distance, 6f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    void CheckConnections()
    {
        if (playerConnections.Count < correctConnections.Count)
            return;

        bool correct = true;

        foreach (var c in playerConnections)
        {
            if (!correctConnections.Contains(c))
            {
                correct = false;
                break;
            }
        }

        if (correct)
            Success();
        else
            Fail();
    }

    void Success()
    {
        Debug.Log("✅ HACK SUCCESS");

        var cams = FindObjectsOfType<SurveillanceCamera>();
        foreach (var cam in cams)
            cam.DisableCamera();

        CloseUI();
    }

    void Fail()
    {
        attempts++;

        Debug.Log("❌ Wrong wiring");

        if (attempts >= maxAttempts)
        {
            Debug.Log("🚨 ALARM TRIGGERED");
            FindObjectOfType<AlarmSystem>()?.TriggerAlarm();
            CloseUI();
            return;
        }

        ResetPuzzle();
    }

    void ResetPuzzle()
    {
        playerConnections.Clear();

        foreach (Transform child in hackPanel.transform)
        {
            if (child.name.Contains("UILine"))
                Destroy(child.gameObject);
        }
    }

    // 🔥 OPEN UI (NO TIMESCALE FREEZE)
    public void OpenUI()
    {
        hackPanel.SetActive(true);

        // DO NOT pause time → keeps audio working
        Time.timeScale = 1f;

        // Disable player movement instead
        if (playerController != null)
            playerController.enabled = false;
    }

    // 🔥 CLOSE UI
    public void CloseUI()
    {
        hackPanel.SetActive(false);

        // Re-enable player
        if (playerController != null)
            playerController.enabled = true;

        ResetPuzzle();
    }
}