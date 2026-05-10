using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public bool hasPhone = false;

    // 🔥 Encryption key
    public bool hasEncryptionKey = false;

    // 🔥 Keycards
    private HashSet<string> keycards = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddKeycard(string id)
    {
        keycards.Add(id);
    }

    public bool HasKeycard(string id)
    {
        return keycards.Contains(id);
    }
}