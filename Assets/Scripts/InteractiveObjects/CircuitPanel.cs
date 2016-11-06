using UnityEngine;
using System.Collections;
using Actors;
using ItemHandler;

[RequireComponent(typeof(BoxCollider2D))]
public class CircuitPanel : MonoBehaviour
{
    PlayerActor player;

    [SerializeField]
    ItemData circuitItem;
    [SerializeField]
    GameObject[] enable;
    [SerializeField]
    GameObject disable;

    bool wasActivated = false;

    void Awake()
    {
        player = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
    }

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

            foreach(GameObject enableGO in enable)
            {
                enableGO.SetActive(true);
            }
            
            disable.SetActive(false);
            


        }
    }
}
