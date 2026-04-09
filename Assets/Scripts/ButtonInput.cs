using UnityEngine;

public class ButtonInput : MonoBehaviour
{
    public static float Horizontal = 0f;
    public static float Vertical = 0f;

    public void PressUp() => Vertical = 1;
    public void PressDown() => Vertical = -1;
    public void PressLeft() => Horizontal = -1;
    public void PressRight() => Horizontal = 1;

    public void ReleaseVertical() => Vertical = 0;
    public void ReleaseHorizontal() => Horizontal = 0;
}