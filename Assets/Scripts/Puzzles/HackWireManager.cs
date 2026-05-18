using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HackWireManager : MonoBehaviour
{
    public GameObject hackPanel;
    public GameObject linePrefab;

    private HackNodeUI startNode;
    private GameObject currentLine;

    // Question → Answer mapping
    public List<Vector2Int> correctConnections = new List<Vector2Int>()
    {
        new Vector2Int(0, 2),
        new Vector2Int(1, 1),
        new Vector2Int(2, 0)
    };

    private List<Vector2Int> playerConnections = new List<Vector2Int>();
    private HashSet<HackNodeUI> usedNodes = new HashSet<HackNodeUI>();

    public int maxAttempts = 2;
    private int attempts = 0;

    private MonoBehaviour playerController;
    private AlarmSystem cachedAlarm;
    private SurveillanceCamera[] cachedCameras;

    private bool isHackCompleted = false;

    [Header("UI Messages")]
    public TMP_Text hackMessageText;
    public float messageDisplayTime = 5f;

    [Header("Intro Settings")]
    public float introDelay = 5f;

    private Coroutine messageCoroutine;

    void Start()
    {
        playerController = FindObjectOfType<PlayerMove>();
        cachedAlarm = FindObjectOfType<AlarmSystem>();
        cachedCameras = FindObjectsOfType<SurveillanceCamera>();

        // Ensure panel starts hidden
        if (hackPanel != null)
            hackPanel.SetActive(false);
    }

    void Update()
    {
        if (currentLine != null && startNode != null)
        {
            Vector3 startScreen = RectTransformUtility.WorldToScreenPoint(null, startNode.transform.position);
            UpdateLine(startScreen, Input.mousePosition);
        }
    }

    // ---------------- CONNECTION ----------------

    public void StartConnection(HackNodeUI node)
    {
        if (isHackCompleted) return;
        if (usedNodes.Contains(node)) return;

        if (currentLine != null)
            Destroy(currentLine);

        startNode = node;
        currentLine = Instantiate(linePrefab, hackPanel.transform);
    }

    public void EndConnection(HackNodeUI endNode)
    {
        if (isHackCompleted) return;
        if (startNode == null || currentLine == null) return;

        if (usedNodes.Contains(endNode))
        {
            CancelPendingConnection();
            return;
        }

        Vector3 startScreen = RectTransformUtility.WorldToScreenPoint(null, startNode.transform.position);
        Vector3 endScreen = RectTransformUtility.WorldToScreenPoint(null, endNode.transform.position);

        UpdateLine(startScreen, endScreen);

        playerConnections.Add(new Vector2Int(startNode.nodeID, endNode.nodeID));

        usedNodes.Add(startNode);
        usedNodes.Add(endNode);

        startNode = null;
        currentLine = null;

        CheckConnections();
    }

    private void CancelPendingConnection()
    {
        if (currentLine != null)
            Destroy(currentLine);

        currentLine = null;
        startNode = null;
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

    // ---------------- CHECK ----------------

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

    // ---------------- RESULT ----------------

    void Success()
    {
        isHackCompleted = true;

        Debug.Log("✅ Hack successful!");

        FindObjectOfType<CutsceneController>().StopHackSequence();

        if (cachedCameras != null)
        {
            foreach (var cam in cachedCameras)
                if (cam != null) cam.DisableCamera();
        }

        ShowMessage("✅ Hack Successful!");

        Invoke(nameof(CloseUI), messageDisplayTime);

        TriggerShowButton[] triggers = FindObjectsOfType<TriggerShowButton>();
        foreach (var t in triggers)
            t.gameObject.SetActive(false);
    }

    void Fail()
    {
        attempts++;

        if (attempts >= maxAttempts)
        {
            ShowMessage("❌ Wrong Answers!\n🚨 Alarm Triggered");

            if (cachedAlarm != null)
                cachedAlarm.TriggerAlarm();

            Invoke(nameof(CloseUI), messageDisplayTime);
            return;
        }

        ShowMessage("❌ Incorrect Match! Try again.");
        ResetPuzzle();
    }

    void ResetPuzzle()
    {
        playerConnections.Clear();
        usedNodes.Clear();
        CancelPendingConnection();

        foreach (Transform child in hackPanel.transform)
        {
            if (child.name.Contains("UILine"))
                Destroy(child.gameObject);
        }
    }

    // ---------------- UI FLOW ----------------

    public void OpenUI()
    {
        if (isHackCompleted) return;

        StartCoroutine(OpenUISequence());
    }

    IEnumerator OpenUISequence()
    {
        if (playerController != null)
            playerController.enabled = false;

        FindObjectOfType<CutsceneController>().StartHackSequence();

        // Show intro message
        ShowMessage("Match each cybersecurity question with the correct answer");

        // Wait before showing puzzle
        yield return new WaitForSecondsRealtime(introDelay);

        // Now show nodes
        hackPanel.SetActive(true);

        Time.timeScale = 1f;
    }

    public void CloseUI()
    {
        hackPanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        ResetPuzzle();
    }

    // ---------------- MESSAGE ----------------

    void ShowMessage(string message)
    {
        if (hackMessageText == null) return;

        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(ShowMessageRoutine(message));
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        hackMessageText.gameObject.SetActive(true);
        hackMessageText.text = message;

        yield return new WaitForSecondsRealtime(messageDisplayTime);

        hackMessageText.gameObject.SetActive(false);

        messageCoroutine = null;
    }

    public bool IsHackCompleted()
    {
        return isHackCompleted;
    }
}