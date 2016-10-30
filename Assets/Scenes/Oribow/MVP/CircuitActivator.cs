using UnityEngine;
using System.Collections;
using Actors;
using ItemHandler;

[RequireComponent(typeof(Collider2D))]
public class CircuitActivator : MonoBehaviour
{
    [SerializeField]
    ItemData circuitItem;
    [SerializeField]
    GameObject toBeActivated;

    bool wasActivated = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (wasActivated)
            return;

        BasicEntityWithEquipmentActor entityActor = col.GetComponentInChildren<BasicEntityWithEquipmentActor>();
        if (entityActor == null)
            return;

        int circuitPos = entityActor.MultiSlotsInventory.GetInv(ItemHandler.PlayerInventory.InventoryType.Main).FindItem(circuitItem.itemID);
        if (circuitPos != -1)
        {
            entityActor.MultiSlotsInventory.GetInv(ItemHandler.PlayerInventory.InventoryType.Main).DropFromInventory(circuitPos);
            wasActivated = true;
            toBeActivated.SetActive(true);
        }
    }
}
