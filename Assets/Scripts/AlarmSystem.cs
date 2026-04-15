using System.Collections;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    public float alarmDuration = 10f;

    [Header("Audio")]
    public AudioSource alarmAudioSource;
    public AudioClip alarmClip;

    private bool isAlarmActive = false;
    private EnemyAI[] cachedEnemies;

    void Start()
    {
        cachedEnemies = FindObjectsOfType<EnemyAI>();
    }

    public void TriggerAlarm()
    {
        if (isAlarmActive) return;

        if (GameplaySessionLogger.Instance != null)
            GameplaySessionLogger.Instance.RegisterAlarmTriggered();

        StartCoroutine(AlarmRoutine());
    }

    public void StopAlarm()
    {
        StopAllCoroutines();
        if (alarmAudioSource != null)
            alarmAudioSource.Stop();
        if (cachedEnemies != null)
        {
            foreach (EnemyAI e in cachedEnemies)
                if (e != null) e.SetAlert(false);
        }
        isAlarmActive = false;
    }

    IEnumerator AlarmRoutine()
    {
        isAlarmActive = true;

        if (alarmAudioSource != null && alarmClip != null)
        {
            alarmAudioSource.loop = true;
            alarmAudioSource.clip = alarmClip;
            alarmAudioSource.Play();
        }

        if (cachedEnemies != null)
        {
            foreach (EnemyAI e in cachedEnemies)
                if (e != null) e.SetAlert(true);
        }

        yield return new WaitForSecondsRealtime(alarmDuration);

        if (cachedEnemies != null)
        {
            foreach (EnemyAI e in cachedEnemies)
                if (e != null) e.SetAlert(false);
        }

        if (alarmAudioSource != null)
            alarmAudioSource.Stop();

        isAlarmActive = false;
    }
}
