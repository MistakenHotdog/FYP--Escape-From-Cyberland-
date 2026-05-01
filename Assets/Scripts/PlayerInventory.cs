using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public bool hasPhone = false;

    private void Awake()
    {
        Instance = this;
    }
}