using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Devices")]
    public bool hasPhone = false;

    [Header("Encryption")]
    public bool hasEncryptionKey = false;

    [Header("Keycards")]
    public bool hasLevel2Keycard = false;

    private void Awake()
    {
        Instance = this;
    }
}