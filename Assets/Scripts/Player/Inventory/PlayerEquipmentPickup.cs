using UnityEngine;
using System.Collections;
using Actors;


namespace ItemHandler
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerEquipmentPickup : MonoBehaviour
    {

        [AssignActorAutomaticly]
        ItemActor actor;

        PlayerActor playerActor;

        private void Item_ItemDropHandler(IInventory inv)
        {
            this.enabled = true;
        }

        void Start()
        {
            playerActor = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
            actor.Item.ItemDropHandler += Item_ItemDropHandler;
        }

        void OnTriggerStay2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                if (playerActor.Inventory.CouldAddItem(actor.Item) && !playerActor.PlayerLimitationHandler.AreAnimationTriggerLocked())
                {
                    playerActor.PlayerLimitationHandler.LockAnimaionTrigger(true);
                    playerActor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.BlockMovement, PlayerLimitation.BlockNonMovement, PlayerLimitation.ResetMovementInput);
                    playerActor.CC2DMotor.frontAnimator.SetTrigger("PickUp");

                    playerActor.PlayerAnimationEventGrabberFront.PickUpReachedItemHandler += PlayerAnimationEventGrabberFront_PickUpReachedItemHandler;
                    playerActor.PlayerAnimationBaseLayerEnd.PickUpFinishedHandler += PlayerAnimationEventGrabberFront_PickUpFinishedHandler;
                }
            }
        }

        private void PlayerAnimationEventGrabberFront_PickUpReachedItemHandler()
        {
            transform.position = playerActor.PlayerEquipmentManager.equipSpawnPoint.position;
            transform.rotation = playerActor.PlayerEquipmentManager.equipSpawnPoint.rotation;
            transform.parent = playerActor.PlayerEquipmentManager.equipSpawnPoint;
            playerActor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.NoLimitation, PlayerLimitation.BlockNonMovement);
            playerActor.PlayerAnimationEventGrabberFront.PickUpReachedItemHandler -= PlayerAnimationEventGrabberFront_PickUpReachedItemHandler;
        }

        private void PlayerAnimationEventGrabberFront_PickUpFinishedHandler()
        {
            playerActor.Inventory.AddItem(actor.Item, null);
            playerActor.PlayerAnimationBaseLayerEnd.PickUpFinishedHandler -= PlayerAnimationEventGrabberFront_PickUpFinishedHandler;
            playerActor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.NoLimitation);
            gameObject.SetActive(true);
            playerActor.PlayerEquipmentManager.ForceEquipItem(actor.Item, gameObject);
            playerActor.PlayerLimitationHandler.LockAnimaionTrigger(false);
        }
    }
}
