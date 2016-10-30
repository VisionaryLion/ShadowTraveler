using UnityEngine;
using System.Collections;
using Actors;


namespace ItemHandler
{
    [RequireComponent(typeof(Collider2D))]
    public class EquipmentPickup : MonoBehaviour
    {
        [AssignActorAutomaticly, SerializeField, HideInInspector]
        ItemActor actor;

        [SerializeField]
        Collider2D triggerCollider;
        int handIndex;

        BasicEntityWithEquipmentActor entityActor;

        private void Item_ItemDropHandler(IInventory inv)
        {
            this.enabled = true;
        }

        void Start()
        {
            actor.Item.ItemDropHandler += Item_ItemDropHandler;
        }

        void OnTriggerStay2D(Collider2D col)
        {
            if (entityActor != null)
                return;

            entityActor = col.GetComponentInChildren<BasicEntityWithEquipmentActor>();

            if (entityActor != null)
            {
                if (entityActor.MultiSlotsInventory.CouldAddItem(actor.Item) && entityActor.AnimationHandler.CanAquireAnyStateTransitionPriority(0, 2))
                {
                    entityActor.AnimationHandler.SetAnyStateTransitionPriority(0, 2);
                    entityActor.SetBlockAllInput(true);
                    entityActor.CC2DMotor.FreezeAndResetMovement(true);
                    entityActor.Animator.SetTrigger("PickUp");
                    entityActor.AnimationHandler.StartListenToAnimationEvent("PickUpReachedItem", new AnimationHandler.AnimationEvent(PickUpReachedItemHandler));
                    entityActor.AnimationHandler.StartListenToAnimationEnd("Item_Pick_up_Anim", new AnimationHandler.AnimationEvent(PickUpFinishedHandler));
                    triggerCollider.enabled = false;
                    this.enabled = false;
                }
            }
        }

        private void PickUpReachedItemHandler()
        {
            handIndex = entityActor.EquipmentManager.GetFreeHand();
            Transform spawnPoint = entityActor.EquipmentManager.GetEquipmentPos(handIndex);
            actor.transform.position = spawnPoint.position;
            actor.transform.rotation = spawnPoint.rotation;
            entityActor.EquipmentManager.ApplyOffset(handIndex, actor.Item.ItemId, actor.transform);
            actor.transform.parent = spawnPoint;
            entityActor.SetBlockAllInput(false);
            entityActor.CC2DMotor.FreezeAndResetMovement(false);
            entityActor.SetBlockAllNonMovement(true);
        }

        private void PickUpFinishedHandler()
        {
            entityActor.AnimationHandler.ResetAnyStateTransitionPriority(0);
            entityActor.SetBlockAllNonMovement(false);
            if (actor.Item.IsEquipment)
            {
                entityActor.MultiSlotsInventory.AddEquipment(actor.Item, null);
                actor.gameObject.SetActive(true);
                entityActor.EquipmentManager.EquipItem(handIndex, actor.Item, actor.gameObject);
            }
            else
            {
                entityActor.MultiSlotsInventory.AddItem(PlayerInventory.InventoryType.Main, actor.Item, actor.gameObject);
            }
            entityActor = null;
        }
    }
}
