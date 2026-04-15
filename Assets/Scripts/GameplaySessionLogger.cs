using System;
using System.IO;
using UnityEngine;

public class GameplaySessionLogger : MonoBehaviour
{
    public static GameplaySessionLogger Instance { get; private set; }

    private const string UI_TYPE_KEY = "UIType";

    private float sessionStartTime;
    private int alarmTriggerCount = 0;
    private int bugTriggerCount = 0;

    private bool sessionEnded = false;
    private string uiControlChoice = "Unknown";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        sessionStartTime = Time.realtimeSinceStartup;
        uiControlChoice = GetUIControlChoice();

        Debug.Log("[SessionLogger] Persistent path = " + Application.persistentDataPath);
        Debug.Log("[SessionLogger] Downloads path = " + GetSaveDirectory());
    }

    private string GetUIControlChoice()
    {
        int uiType = PlayerPrefs.GetInt(UI_TYPE_KEY, 1);

        switch (uiType)
        {
            case 1: return "joystick";
            case 2: return "buttons";
            case 3: return "voice";
            default: return "unknown";
        }
    }

    private string GetSaveDirectory()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return "/storage/emulated/0/Download";
#else
        return Application.persistentDataPath;
#endif
    }

    public void RegisterAlarmTriggered()
    {
        alarmTriggerCount++;
        Debug.Log("[SessionLogger] Alarm triggered. Count = " + alarmTriggerCount);
    }

    public void RegisterBugTriggered()
    {
        bugTriggerCount++;
        Debug.Log("[SessionLogger] Bug triggered. Count = " + bugTriggerCount);
    }

    public void EndSession(bool completed)
    {
        if (sessionEnded) return;

        sessionEnded = true;

        float completionTime = Time.realtimeSinceStartup - sessionStartTime;
        string completionText = completed ? "Yes" : "No";

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{uiControlChoice}_{timestamp}.csv";

        string saveDirectory = GetSaveDirectory();
        string filePath = Path.Combine(saveDirectory, fileName);

        try
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("UIControlChoice,GameCompletion,GameCompletionTimeSeconds,AlarmTriggerCount,BugTriggerCount");
                writer.WriteLine($"{uiControlChoice},{completionText},{completionTime:F2},{alarmTriggerCount},{bugTriggerCount}");
            }

            Debug.Log("[SessionLogger] CSV saved to: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("[SessionLogger] Failed to save CSV to Downloads: " + e.Message);

            // Fallback to persistentDataPath
            try
            {
                string fallbackPath = Path.Combine(Application.persistentDataPath, fileName);

                using (StreamWriter writer = new StreamWriter(fallbackPath, false))
                {
                    writer.WriteLine("UIControlChoice,GameCompletion,GameCompletionTimeSeconds,AlarmTriggerCount,BugTriggerCount");
                    writer.WriteLine($"{uiControlChoice},{completionText},{completionTime:F2},{alarmTriggerCount},{bugTriggerCount}");
                }

                Debug.Log("[SessionLogger] Fallback CSV saved to: " + fallbackPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("[SessionLogger] Fallback save also failed: " + ex.Message);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (!sessionEnded)
        {
            EndSession(false);
        }
    }
}