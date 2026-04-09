using System.Collections;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    public float alarmDuration = 10f;

    [Header("Audio")]
    public AudioSource alarmAudioSource;
    public AudioClip alarmClip;

    private bool isAlarmActive = false;

    public void TriggerAlarm()
    {
        if (isAlarmActive) return;

        StartCoroutine(AlarmRoutine());
    }

    IEnumerator AlarmRoutine()
    {
        isAlarmActive = true;

        Debug.Log("🚨 ALARM TRIGGERED");

        // 🔊 PLAY SOUND
        if (alarmAudioSource != null && alarmClip != null)
        {
            alarmAudioSource.loop = true;
            alarmAudioSource.clip = alarmClip;
            alarmAudioSource.Play();
        }

        // Alert enemies
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI e in enemies)
        {
            if (e != null)
                e.SetAlert(true);
        }

        yield return new WaitForSeconds(alarmDuration);

        // Stop alarm
        foreach (EnemyAI e in enemies)
        {
            if (e != null)
                e.SetAlert(false);
        }

        // 🔇 STOP SOUND
        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();
        }

        Debug.Log("✅ Alarm Ended");

        isAlarmActive = false;
    }
}