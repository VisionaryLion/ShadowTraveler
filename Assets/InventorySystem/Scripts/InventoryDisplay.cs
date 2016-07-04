
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]//Backend script essential

public class InventoryDisplay : MonoBehaviour {
    Inventory invObject;

    public GameObject inventoryPanel;
    public GameObject slotPanel;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public List<GameObject> slotDisplay = new List<GameObject>();

    void Awake()
    {
        if (!inventoryPanel)
            inventoryPanel = GameObject.Find("Inventory panel");
        if (!slotPanel)
            slotPanel = inventoryPanel.transform.FindChild("Slot panel").gameObject;
        if(!invObject)
            invObject = gameObject.GetComponent<Inventory>();
    }

    void Start() 
    {
        for(int i=0; i<invObject.inventorySize; i++)
        {
            slotDisplay.Add(Instantiate(inventorySlot));
            slotDisplay[i].transform.SetParent(slotPanel.transform);
            slotDisplay[i].GetComponent<SlotHandler>().SlotID = i;      //TODO: Add validation
        }
    }    
	
    public GameObject FindEmptySlot()
    {
        for(int i=0;i<slotDisplay.Count;i++)
        {
            if(!slotDisplay[i])
            {
                Debug.LogError("Couldnt dereference slotlist element at I:" + i);
            }
            if(slotDisplay[i].GetComponent<RectTransform>().childCount == 0)
            {
                return slotDisplay[i];
            }
        }
        Debug.LogError("Couldnt find an empty slot");
        return null;
    }

    public void AddItemToSlot(GameObject slot, ItemInterface itemIntf)
    {
        GameObject newItem = Instantiate(inventoryItem);
        newItem.transform.SetParent(slot.transform);
        newItem.transform.localPosition = Vector3.zero;
        newItem.GetComponent<DisplayItem>().itemInterface = itemIntf;
        SlotHandler slH = slot.GetComponent<SlotHandler>();
        if (!slH)
            Debug.LogError("Couldnt find script attached to slot");
        slH.heldItem = newItem;
    }

}
