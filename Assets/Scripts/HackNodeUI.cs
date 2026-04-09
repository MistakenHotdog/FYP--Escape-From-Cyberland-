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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isStartNode)
        {
            manager.StartConnection(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isStartNode)
        {
            manager.EndConnection(this);
        }
    }
}