using UnityEngine;

public static class VoiceMotor
{
    public static float Horizontal { get; private set; }
    public static float Vertical { get; private set; }

    private static float holdUntil = 0f;

    public static bool HasInput =>
        Mathf.Abs(Horizontal) > 0.01f || Mathf.Abs(Vertical) > 0.01f;

    public static void Move(float h, float v, float duration)
    {
        Horizontal = Mathf.Clamp(h, -1f, 1f);
        Vertical = Mathf.Clamp(v, -1f, 1f);
        holdUntil = Time.time + Mathf.Max(0.05f, duration);
    }

    public static void Stop()
    {
        Horizontal = 0f;
        Vertical = 0f;
        holdUntil = 0f;
    }

    public static void Tick()
    {
        if (HasInput && Time.time >= holdUntil)
            Stop();
    }
}