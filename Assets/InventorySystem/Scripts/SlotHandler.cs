//Add to UI slot prefab
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
            Debug.Log("Item dropped on empty slot:"+SlotID);
            droppedItem.slotToMoveTo = gameObject;
            droppedItem.originalParentSlot.GetComponent<SlotHandler>().heldItem = null;
            heldItem = droppedItem.gameObject;
        }
        else
        {
            //GameObject toMove = heldItem;
            droppedItem.slotToMoveTo = gameObject;

            Debug.Log("Moving item:" + heldItem.name + " to slot:" + droppedItem.originalParentSlot.GetComponent<SlotHandler>().SlotID);
            heldItem.transform.SetParent(droppedItem.originalParentSlot.transform);
            heldItem.transform.position = droppedItem.originalParentSlot.transform.position;

            droppedItem.originalParentSlot.GetComponent<SlotHandler>().heldItem = heldItem;
            heldItem = droppedItem.gameObject;
        }
    }
}
