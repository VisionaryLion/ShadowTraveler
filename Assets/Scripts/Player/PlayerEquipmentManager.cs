using UnityEngine;
using Actors;
using System.Collections;
using System;

namespace ItemHandler
{
    public class PlayerEquipmentManager : MonoBehaviour
    {

        [SerializeField]
        EquipmentButtonBinding[] binds; //according to keyboard numbers
        [SerializeField]
        public Transform equipSpawnPoint;

        public bool allowInput;

        [AssignActorAutomaticly]
        [SerializeField]
        [HideInInspector]
        PlayerActor actor;

        GameObject currentPhysicalEquipment;
        IItem currentEquipedItem;
        EquipmentButtonBinding currentBind;

        // Update is called once per frame
        void Update()
        {
            if (!allowInput)
                return;
            foreach (EquipmentButtonBinding b in binds)
            {
                if (currentBind == b)
                    continue;
                if (Input.GetKeyDown(b.key) && actor.Inventory.ContainsItem(b.equipment.itemID) && !actor.PlayerLimitationHandler.AreAnimationTriggerLocked())
                {
                    if (currentEquipedItem != null)
                    {
                        actor.Inventory.AddGameObjectCopyOfItem(currentEquipedItem, currentPhysicalEquipment);
                    }
                    if (currentPhysicalEquipment != null)
                        currentPhysicalEquipment.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);
                    int itemIndex = actor.Inventory.FindItem(b.equipment.itemID);
                    currentEquipedItem = actor.Inventory.GetItem(itemIndex);
                    currentPhysicalEquipment = actor.Inventory.EquipItem(itemIndex);
                    currentPhysicalEquipment.transform.position = equipSpawnPoint.position;
                    currentPhysicalEquipment.transform.rotation = equipSpawnPoint.rotation;
                    currentPhysicalEquipment.transform.parent = equipSpawnPoint;
                    currentBind = b;

                    actor.PlayerAnimationUpperBodyEnd.EquipFinishedHandler += PlayerAnimationEventGrabberFront_EquipFinishedHandler;
                    //actor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.BlockEquipmentUse);
                    actor.CC2DMotor.frontAnimator.SetTrigger("EquipItem");
                }
            }
        }

        private void PlayerAnimationEventGrabberFront_EquipFinishedHandler()
        {
            actor.PlayerAnimationUpperBodyEnd.EquipFinishedHandler -= PlayerAnimationEventGrabberFront_EquipFinishedHandler;
            currentPhysicalEquipment.SendMessage("OnEquiped", SendMessageOptions.DontRequireReceiver);
            //actor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.NoLimitation);
        }

        public void ForceEquipItem(IItem item, GameObject gameObject)
        {
            if(currentPhysicalEquipment != null)
            currentPhysicalEquipment.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);
            currentPhysicalEquipment = gameObject;
            currentEquipedItem = item;
            currentPhysicalEquipment.transform.position = equipSpawnPoint.position;
            currentPhysicalEquipment.transform.rotation = equipSpawnPoint.rotation;
            currentPhysicalEquipment.transform.parent = equipSpawnPoint;

            foreach (EquipmentButtonBinding b in binds)
            {
                if (b.equipment.itemID == item.ItemId)
                {
                    currentBind = b;
                    break;
                }
            }
            currentPhysicalEquipment.SendMessage("OnEquiped", SendMessageOptions.DontRequireReceiver);
        }

        [Serializable]
        public class EquipmentButtonBinding
        {
            public KeyCode key;
            public ItemData equipment;
        }
    }
}
