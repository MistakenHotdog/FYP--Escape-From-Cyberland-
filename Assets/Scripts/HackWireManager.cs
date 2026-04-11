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

    private MonoBehaviour playerController;
    private AlarmSystem cachedAlarm;
    private SurveillanceCamera[] cachedCameras;

    private bool isHackCompleted = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerMove>();
        cachedAlarm = FindObjectOfType<AlarmSystem>();
        cachedCameras = FindObjectsOfType<SurveillanceCamera>();
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
        if (isHackCompleted) return; // ❌ block

        startNode = node;
        currentLine = Instantiate(linePrefab, hackPanel.transform);
    }

    public void EndConnection(HackNodeUI endNode)
    {
        if (isHackCompleted) return; // ❌ block
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
        isHackCompleted = true;

        if (cachedCameras != null)
        {
            foreach (var cam in cachedCameras)
                if (cam != null) cam.DisableCamera();
        }

        // 🔥 Disable ALL hack triggers in scene
        TriggerShowButton[] triggers = FindObjectsOfType<TriggerShowButton>();
        foreach (var t in triggers)
        {
            t.gameObject.SetActive(false);
        }

        CloseUI();
    }

    void Fail()
    {
        attempts++;

        Debug.Log("❌ Wrong wiring");

        if (attempts >= maxAttempts)
        {
            Debug.Log("🚨 ALARM TRIGGERED");
            if (cachedAlarm != null) cachedAlarm.TriggerAlarm();
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

    public void OpenUI()
    {
        if (isHackCompleted) return; // ❌ block reopening

        hackPanel.SetActive(true);
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = false;
    }

    public void CloseUI()
    {
        hackPanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        ResetPuzzle();
    }

    public bool IsHackCompleted()
    {
        return isHackCompleted;
    }
}