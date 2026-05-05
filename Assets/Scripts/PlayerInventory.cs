using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public bool hasPhone = false;

    // 🔥 KEYCARDS
    private HashSet<string> keycards = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddKeycard(string id)
    {
        keycards.Add(id);
        Debug.Log("🪪 Keycard collected: " + id);
    }

    public bool HasKeycard(string id)
    {
        return keycards.Contains(id);
    }
}