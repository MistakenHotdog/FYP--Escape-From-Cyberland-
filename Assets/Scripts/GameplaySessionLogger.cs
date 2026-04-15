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
    }

    private string GetUIControlChoice()
    {
        int uiType = PlayerPrefs.GetInt(UI_TYPE_KEY, 1);

        switch (uiType)
        {
            case 1: return "Joystick";
            case 2: return "Buttons";
            case 3: return "Voice";
            default: return "Unknown";
        }
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
        string safeUIName = uiControlChoice.ToLower();
        string fileName = $"{safeUIName}_{timestamp}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("UIControlChoice,GameCompletion,GameCompletionTimeSeconds,AlarmTriggerCount,BugTriggerCount");
                writer.WriteLine($"{uiControlChoice},{completionText},{completionTime:F2},{alarmTriggerCount},{bugTriggerCount}");
            }

            Debug.Log("[SessionLogger] CSV saved to: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("[SessionLogger] Failed to save CSV: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        // If player force-closes while still in gameplay, save as incomplete
        if (!sessionEnded)
        {
            EndSession(false);
        }
    }
}