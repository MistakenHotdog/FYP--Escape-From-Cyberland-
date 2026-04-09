using System.Collections;
using UnityEngine;

public class AlarmSystem : MonoBehaviour
{
    public float alarmDuration = 10f;

    private bool isAlarmActive = false;

    public void TriggerAlarm()
    {
        if (isAlarmActive) return;

        StartCoroutine(AlarmRoutine());
    }

    IEnumerator AlarmRoutine()
    {
        isAlarmActive = true;

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        Debug.Log("🚨 ALARM TRIGGERED");

        foreach (EnemyAI e in enemies)
        {
            if (e != null)
                e.SetAlert(true);
        }

        yield return new WaitForSeconds(alarmDuration);

        foreach (EnemyAI e in enemies)
        {
            if (e != null)
                e.SetAlert(false);
        }

        Debug.Log("✅ Alarm Ended");

        isAlarmActive = false;
    }
}