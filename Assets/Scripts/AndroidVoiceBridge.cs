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
        try
        {
            Debug.Log("[Voice] Android bridge StartListening entered");

            bool hasMic = Permission.HasUserAuthorizedPermission(Permission.Microphone);
            Debug.Log("[Voice] Has mic permission = " + hasMic);

            if (!hasMic)
            {
                Debug.Log("[Voice] Requesting microphone permission now");
                Permission.RequestUserPermission(Permission.Microphone);
                return;
            }

            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var plugin = new AndroidJavaObject("com.cyberland.voice.VoiceRecognizerPlugin"))
            {
                Debug.Log("[Voice] Plugin object created successfully");
                plugin.Call("startListening", activity, unityReceiverObjectName);
                Debug.Log("[Voice] startListening call sent to Java");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Voice] Android bridge exception: " + e);
        }
#endif
    }

    public void StopListening()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var plugin = new AndroidJavaObject("com.cyberland.voice.VoiceRecognizerPlugin"))
            {
                plugin.Call("stopListening");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Voice] StopListening exception: " + e);
        }
#endif
    }

    public void DestroyRecognizer()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var plugin = new AndroidJavaObject("com.cyberland.voice.VoiceRecognizerPlugin"))
            {
                plugin.Call("destroy");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Voice] DestroyRecognizer exception: " + e);
        }
#endif
    }
}