using UnityEngine;

public class ControlUIManager : MonoBehaviour
{
    public GameObject joystickUI;
    public GameObject buttonsUI;
    public GameObject voiceUI;

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        int uiType = PlayerPrefs.GetInt("UIType", 1);

        Debug.Log("[UI] Mode = " + uiType);

        joystickUI.SetActive(uiType == 1);
        buttonsUI.SetActive(uiType == 2);
        voiceUI.SetActive(uiType == 3);
    }
}