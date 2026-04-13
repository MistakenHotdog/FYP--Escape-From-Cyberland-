using UnityEngine;

public class ButtonInput : MonoBehaviour
{
    // Read by PlayerMove (same values as Joystick.Horizontal/Vertical)
    public static float Horizontal { get; private set; }
    public static float Vertical { get; private set; }

    // Individual button hold states
    private static bool upHeld, downHeld, leftHeld, rightHeld;

    // ---- PRESS (hook to PointerDown) ----
    public void PressUp()    { upHeld = true;    Recalculate(); }
    public void PressDown()  { downHeld = true;  Recalculate(); }
    public void PressLeft()  { leftHeld = true;  Recalculate(); }
    public void PressRight() { rightHeld = true; Recalculate(); }

    // ---- RELEASE (hook to PointerUp) ----
    public void ReleaseUp()    { upHeld = false;    Recalculate(); }
    public void ReleaseDown()  { downHeld = false;  Recalculate(); }
    public void ReleaseLeft()  { leftHeld = false;  Recalculate(); }
    public void ReleaseRight() { rightHeld = false; Recalculate(); }

    private static void Recalculate()
    {
        Vertical   = (upHeld    ? 1f : 0f) + (downHeld ? -1f : 0f);
        Horizontal = (rightHeld ? 1f : 0f) + (leftHeld ? -1f : 0f);
    }

    // Safety: reset when the button UI is hidden (e.g. switching to Joystick mid-game)
    void OnDisable()
    {
        upHeld = downHeld = leftHeld = rightHeld = false;
        Recalculate();
    }
}
