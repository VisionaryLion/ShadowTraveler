using UnityEngine;
using System.Collections;
using Actors;


namespace ItemHandler
{
    [RequireComponent(typeof(Collider2D))]
    public class EquipmentPickup : MonoBehaviour
    {

        [AssignActorAutomaticly]
        ItemActor actor;

        [SerializeField]
        Collider2D triggerCollider;

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
                }
            }
        }

        private void PickUpReachedItemHandler()
        {
            Transform spawnPoint = entityActor.EquipmentManager.GetFreeHand();
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            transform.parent = spawnPoint;
            entityActor.SetBlockAllInput(false);
            entityActor.CC2DMotor.FreezeAndResetMovement(false);
            entityActor.SetBlockAllNonMovement(true);
        }

        private void PickUpFinishedHandler()
        {
            entityActor.AnimationHandler.ResetAnyStateTransitionPriority(0);
            entityActor.MultiSlotsInventory.AddItem(actor.Item, gameObject);
            entityActor.SetBlockAllNonMovement(false);
            gameObject.SetActive(true);
            entityActor.EquipmentManager.EquipItemRight(actor.Item, gameObject);
        }
    }
}
