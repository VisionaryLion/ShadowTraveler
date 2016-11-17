using UnityEngine;
using System.Collections;
using Actors;


namespace ItemHandler
{
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        [AssignActorAutomaticly, SerializeField, HideInInspector]
        ItemActor actor;

        [SerializeField]
        Collider2D triggerCollider;

        private void Item_ItemDropHandler(IInventory inv)
        {
            this.enabled = true;
        }

        void Start()
        {
            actor.Item.ItemDropHandler += Item_ItemDropHandler;
        }
        
        private void Item_ItemDropHandler(IInventoryEntity equiper, GameObject newItem)
        {
            triggerCollider.enabled = true;
            this.enabled = true;
        }

        void OnTriggerStay2D(Collider2D col)
        {
            IInventoryEntity inventoryEntity = col.GetComponentInChildren<IInventoryEntity>();

            if (inventoryEntity == null)
                return;

            if (inventoryEntity.TryPickItemUp(actor))
            {
                triggerCollider.enabled = false;
                this.enabled = false;
            }
        }
    }
}
