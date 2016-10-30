using UnityEngine;
using Actors;
using System.Collections;
using System;

namespace ItemHandler
{
    public class EquipmentManager : MonoBehaviour
    {
        [AssignActorAutomaticly]
        [SerializeField]
        [HideInInspector]
        BasicEntityWithEquipmentActor actor;

        [SerializeField]
        public Transform equipmentSpawnPointLeft;
        [SerializeField]
        public Transform equipmentSpawnPointRight;

        [SerializeField]
        EquipmentOffset equipmentOffset;

        public IItem CurrentEquipedItemLeft { get { return currentEquipedItemLeft; } }
        public GameObject CurrentEquipedGameObjectLeft { get { return currentEquipedGameObjectLeft; } }
        public IItem CurrentEquipedItemRight { get { return currentEquipedItemRight; } }
        public GameObject CurrentEquipedGameObjectRight { get { return currentEquipedGameObjectRight; } }

        GameObject currentEquipedGameObjectLeft;
        IItem currentEquipedItemLeft;
        GameObject currentEquipedGameObjectRight;
        IItem currentEquipedItemRight;

        private void EquipFinishedLeftHandler()
        {
            currentEquipedGameObjectLeft.SendMessage("OnEquiped", SendMessageOptions.DontRequireReceiver);
            actor.AnimationHandler.ResetAnyStateTransitionPriority(1);
        }

        private void EquipFinishedRightHandler()
        {
            currentEquipedGameObjectRight.SendMessage("OnEquiped", SendMessageOptions.DontRequireReceiver);
            actor.AnimationHandler.ResetAnyStateTransitionPriority(1);
        }

        public void EquipItemLeft(IItem item, GameObject gameObject)
        {
            if (!actor.AnimationHandler.CanAquireAnyStateTransitionPriority(1, 1))
                return;
            if (currentEquipedItemLeft != null)
                currentEquipedGameObjectLeft.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);
            if (currentEquipedItemLeft != null)
                actor.MultiSlotsInventory.AddGameObjectCopyOfItem(PlayerInventory.InventoryType.Left, currentEquipedItemLeft, currentEquipedGameObjectLeft);
            

            currentEquipedGameObjectLeft = gameObject;
            currentEquipedItemLeft = item;

            
            currentEquipedGameObjectLeft.transform.parent = equipmentSpawnPointLeft;
            currentEquipedGameObjectLeft.transform.localScale = Vector3.one;
            currentEquipedGameObjectLeft.transform.localPosition = equipmentOffset.GetOffsetPos(-1, item.ItemId);
            
            currentEquipedGameObjectLeft.transform.localRotation = Quaternion.Euler(0, 0, equipmentSpawnPointLeft.rotation.eulerAngles.z + equipmentOffset.GetOffsetRot(-1, item.ItemId));
            

            actor.Animator.SetTrigger("EquipItem");
            actor.AnimationHandler.SetAnyStateTransitionPriority(1, 1);
            actor.AnimationHandler.StartListenToAnimationEnd("Equip_Item_Anim", new AnimationHandler.AnimationEvent(EquipFinishedLeftHandler));
        }

        public void EquipItemRight(IItem item, GameObject gameObject)
        {
            if (!actor.AnimationHandler.CanAquireAnyStateTransitionPriority(1, 1))
                return;
            if (currentEquipedItemRight != null)
                currentEquipedGameObjectRight.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);
            if (currentEquipedItemRight != null)
                actor.MultiSlotsInventory.AddGameObjectCopyOfItem(PlayerInventory.InventoryType.Right, currentEquipedItemRight, currentEquipedGameObjectRight);
            

            currentEquipedGameObjectRight = gameObject;
            currentEquipedItemRight = item;

            
            currentEquipedGameObjectRight.transform.parent = equipmentSpawnPointRight;
            currentEquipedGameObjectRight.transform.localScale = Vector3.one;
            currentEquipedGameObjectRight.transform.localPosition = equipmentOffset.GetOffsetPos(1, item.ItemId);
            currentEquipedGameObjectRight.transform.localRotation = Quaternion.Euler(0, 0, equipmentSpawnPointRight.rotation.eulerAngles.z + equipmentOffset.GetOffsetRot(1, item.ItemId));
            

            actor.Animator.SetTrigger("EquipItem");
            actor.AnimationHandler.SetAnyStateTransitionPriority(1, 1);
            actor.AnimationHandler.StartListenToAnimationEnd("Equip_Item_Anim", new AnimationHandler.AnimationEvent(EquipFinishedRightHandler));
        }

        public void EquipItem(int handIndex,  IItem item, GameObject gameObject)
        {
            if (handIndex == 1)
                EquipItemRight(item, gameObject);
            else
                EquipItemLeft(item, gameObject);
        }

        public void EquipNextItem(int handIndex)
        {
            Inventory inv = actor.MultiSlotsInventory.GetInv((handIndex == 1) ? PlayerInventory.InventoryType.Right : PlayerInventory.InventoryType.Left);
            if (inv.IsEmpty)
                return;

            GameObject equippedObj = (handIndex == 1) ? currentEquipedGameObjectRight : currentEquipedGameObjectLeft;

            if (inv.FilledSlots == 1 && equippedObj != null)
                return;
            IItem equippedItem = (handIndex == 1) ? currentEquipedItemRight : currentEquipedItemLeft;
            bool earlyOut = false;
            IItem newItem = null;
            IItem cItem;
            int newItemIndex = 0;

            for (int iItem = 0; iItem < inv.InventorySize; iItem++)
            {
                cItem = inv.GetItem(iItem);
                if (cItem == equippedItem)
                {
                    earlyOut = true;
                    continue;
                }
                if (cItem == null)
                    continue;

                newItem = cItem;
                newItemIndex = iItem;
                if (earlyOut)
                {
                    break;
                }
            }
            Debug.Assert(newItem != null);

            EquipItem(handIndex, newItem, inv.DropFromInventorySilent(newItemIndex));
        }

        public Transform GetEquipmentPos (int handIndex)
        {
            if (handIndex == -1)
                return equipmentSpawnPointLeft;
            if (handIndex == 1)
                return equipmentSpawnPointRight;
            return equipmentSpawnPointLeft;
        }

        public void ApplyOffset(int handIndex, int itemId, Transform target)
        {
            target.position = target.position + (Vector3)equipmentOffset.GetOffsetPos(handIndex, itemId);
            target.rotation = Quaternion.Euler(0, 0, target.rotation.eulerAngles.z + equipmentOffset.GetOffsetRot(handIndex, itemId));
        }

        public int GetFreeHand()
        {
            if (currentEquipedGameObjectLeft == null)
                return -1;
            if (currentEquipedGameObjectRight == null)
                return 1;
            return 0;
        }
    }
}
