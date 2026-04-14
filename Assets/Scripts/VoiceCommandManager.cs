using UnityEngine;

public class VoiceCommandManager : MonoBehaviour
{
    private const string UI_TYPE_KEY = "UIType";
    private const int VOICE_UI_TYPE = 3;
    private bool hackAllowed = false;
    private HackWireManager currentHackManager = null;
    [Header("Mode")]
    public bool enableEditorSimulation = true;
    public bool logCommands = true;

    [Header("Movement")]
    public float moveDuration = 0.75f;

    [Header("References")]
    public PausePanelController pausePanelController;
    public GameObject voiceUIPanel;
    public HackWireManager hackWireManager;

    private AndroidVoiceBridge androidBridge;

    void Awake()
    {
        androidBridge = new AndroidVoiceBridge(gameObject.name);
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (enableEditorSimulation && IsVoiceModeActive())
        {
            SimulateEditorCommands();
        }
#endif
    }

    public bool IsVoiceModeActive()
    {
        int uiType = PlayerPrefs.GetInt(UI_TYPE_KEY, 1);
        if (uiType != VOICE_UI_TYPE)
            return false;

        if (voiceUIPanel != null && !voiceUIPanel.activeInHierarchy)
            return false;

        return true;
    }
    public void SetHackAllowed(bool allowed, HackWireManager manager)
    {
        hackAllowed = allowed;
        currentHackManager = manager;
    }
    public void StartListening()
    {
        if (!IsVoiceModeActive())
        {
            if (logCommands)
                Debug.Log("[Voice] Ignored StartListening because voice UI is not active.");
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        androidBridge.StartListening();
#else
        if (logCommands)
            Debug.Log("[Voice] Editor test mode active. Use I/K/J/L/P/H keys.");
#endif
    }

    public void StopListening()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        androidBridge.StopListening();
#endif
        VoiceMotor.Stop();
    }

    public void OnVoiceResult(string recognizedText)
    {
        if (!IsVoiceModeActive())
            return;

        if (string.IsNullOrWhiteSpace(recognizedText))
            return;

        string cmd = recognizedText.Trim().ToLowerInvariant();

        if (logCommands)
            Debug.Log("[Voice] Heard: " + cmd);

        ExecuteCommand(cmd);
    }

    public void OnVoiceError(string error)
    {
        if (logCommands)
            Debug.LogWarning("[Voice] " + error);
    }

    public void ExecuteCommand(string cmd)
    {
        if (!IsVoiceModeActive())
            return;

        cmd = cmd.Replace("-", " ").Trim();

        if (cmd.Contains("forward") || cmd == "go forward")
        {
            VoiceMotor.Move(0f, 1f, moveDuration);
            return;
        }

        if (cmd.Contains("back") || cmd.Contains("backward") || cmd == "go back")
        {
            VoiceMotor.Move(0f, -1f, moveDuration);
            return;
        }

        if (cmd == "left" || cmd.Contains("go left"))
        {
            VoiceMotor.Move(-1f, 0f, moveDuration);
            return;
        }

        if (cmd == "right" || cmd.Contains("go right"))
        {
            VoiceMotor.Move(1f, 0f, moveDuration);
            return;
        }

        if (cmd.Contains("pause"))
        {
            VoiceMotor.Stop();

            if (pausePanelController != null)
                pausePanelController.PauseGame();

            return;
        }

        if (cmd.Contains("hack"))
        {
            VoiceMotor.Stop();

            if (hackAllowed && currentHackManager != null && !currentHackManager.IsHackCompleted())
            {
                currentHackManager.OpenUI();
            }
            else if (logCommands)
            {
                Debug.Log("[Voice] Hack command ignored: player is not in a valid hack trigger.");
            }

            return;
        }
        if (cmd.Contains("stop"))
        {
            VoiceMotor.Stop();
        }
    }

    private void SimulateEditorCommands()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ExecuteCommand("forward");

        if (Input.GetKeyDown(KeyCode.K))
            ExecuteCommand("backward");

        if (Input.GetKeyDown(KeyCode.J))
            ExecuteCommand("left");

        if (Input.GetKeyDown(KeyCode.L))
            ExecuteCommand("right");

        if (Input.GetKeyDown(KeyCode.P))
            ExecuteCommand("pause");

        if (Input.GetKeyDown(KeyCode.H))
            ExecuteCommand("hack");

        if (Input.GetKeyDown(KeyCode.Space))
            ExecuteCommand("stop");
    }

    void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        androidBridge.DestroyRecognizer();
#endif
    }
}