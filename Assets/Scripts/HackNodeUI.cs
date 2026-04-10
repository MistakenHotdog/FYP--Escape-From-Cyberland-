using UnityEngine;
using UnityEngine.EventSystems;

public class HackNodeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int nodeID;
    public bool isStartNode;

    private HackWireManager manager;

    void Start()
    {
        manager = FindObjectOfType<HackWireManager>();
        if (manager == null)
            Debug.LogWarning($"[HackNodeUI] {name}: No HackWireManager found in scene.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isStartNode && manager != null)
        {
            manager.StartConnection(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isStartNode && manager != null)
        {
            manager.EndConnection(this);
        }
    }
}
