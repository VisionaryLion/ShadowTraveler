using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SlotHandler : MonoBehaviour, IDropHandler {

    public int SlotID;
    public GameObject heldItem;

    void Awake()
    {
        heldItem = null;
        SlotID = -1;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DisplayItem droppedItem = eventData.pointerDrag.GetComponent<DisplayItem>(); 
        if(heldItem == null)            //Slot is empty
        {
            droppedItem.slotToMoveTo = gameObject;
            droppedItem.originalParentSlot.GetComponent<SlotHandler>().heldItem = null;
        }
        else
        {
            GameObject toMove = heldItem;
            droppedItem.slotToMoveTo = gameObject;
            
            heldItem.transform.parent = droppedItem.originalParentSlot.transform;
            heldItem.transform.position = droppedItem.originalParentSlot.transform.position;

            droppedItem.originalParentSlot.GetComponent<SlotHandler>().heldItem = heldItem;
            heldItem = droppedItem.gameObject;
        }
    }
}
