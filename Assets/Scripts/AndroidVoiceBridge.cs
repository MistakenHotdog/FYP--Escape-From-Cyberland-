using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class AndroidVoiceBridge
{
    private readonly string unityReceiverObjectName;

    public AndroidVoiceBridge(string receiverObjectName)
    {
        unityReceiverObjectName = receiverObjectName;
    }

    public void StartListening()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            return;
        }

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var plugin = new AndroidJavaObject("com.cyberland.voice.VoiceRecognizerPlugin"))
        {
            plugin.Call("startListening", activity, unityReceiverObjectName);
        }
#endif
    }

    public void StopListening()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var plugin = new AndroidJavaObject("com.cyberland.voice.VoiceRecognizerPlugin"))
        {
            plugin.Call("stopListening");
        }
#endif
    }

    public void DestroyRecognizer()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var plugin = new AndroidJavaObject("com.cyberland.voice.VoiceRecognizerPlugin"))
        {
            plugin.Call("destroy");
        }
#endif
    }
}