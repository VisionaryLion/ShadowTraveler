using UnityEngine;
using System.Collections;
using Actors;


namespace ItemHandler
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerPickup : MonoBehaviour
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

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                if (playerActor.Inventory.CouldAddItem(actor.Item))
                {
                    playerActor.PlayerLimitationHandler.SetLimitation(PlayerLimitationHandler.PlayerLimition.BlockPlayerInput);
                    playerActor.CC2DMotor.frontAnimator.SetTrigger("PickUp");

                    playerActor.PlayerAnimationEventGrabberFront.PickUpReachedItemHandler += PlayerAnimationEventGrabberFront_PickUpReachedItemHandler;
                    playerActor.PlayerAnimationEventGrabberFront.PickUpFinishedHandler += PlayerAnimationEventGrabberFront_PickUpFinishedHandler;
                }
            }
        }

        private void PlayerAnimationEventGrabberFront_PickUpReachedItemHandler()
        {
            transform.position = playerActor.PlayerEquipmentManager.equipSpawnPoint.position;
            transform.rotation = playerActor.PlayerEquipmentManager.equipSpawnPoint.rotation;
            transform.parent = playerActor.PlayerEquipmentManager.equipSpawnPoint;
            playerActor.PlayerLimitationHandler.SetLimitation(PlayerLimitationHandler.PlayerLimition.BlockNonMovement);
    }

        private void PlayerAnimationEventGrabberFront_PickUpFinishedHandler()
        {
            playerActor.Inventory.AddItem(actor.Item, gameObject);
            playerActor.PlayerAnimationEventGrabberFront.PickUpFinishedHandler -= PlayerAnimationEventGrabberFront_PickUpFinishedHandler;
            playerActor.PlayerLimitationHandler.SetLimitation(PlayerLimitationHandler.PlayerLimition.NoLimitation);
        }
    }
}
