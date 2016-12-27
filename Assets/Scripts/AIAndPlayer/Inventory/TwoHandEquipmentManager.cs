using UnityEngine;
using Entities;
using System.Collections;
using System;

namespace ItemHandler
{
    public class TwoHandEquipmentManager : MonoBehaviour
    {
        [AssignEntityAutomaticly]
        [SerializeField]
        [HideInInspector]
        ActingEquipmentEntity actor;

        [SerializeField]
        public Transform equipmentSpawnPointLeft;
        [SerializeField]
        public Transform equipmentSpawnPointRight;
        [SerializeField]
        EquipmentOffset equipmentOffset;


        public IItem CurrentEquipedItemLeft { get { return leftHandItemActor.Item; } }
        public IItem CurrentEquipedItemRight { get { return rightHandItemActor.Item; } }

        TwoHandItemEntity leftHandItemActor;
        int leftHandItemIndex;

        TwoHandItemEntity rightHandItemActor;
        int rightHandItemIndex;

        TwoHandItemEntity itemInProccessOfPicking;
        int targetedHand;

        bool isInProcessOfEquiping = false;

        void Awake()
        {

            // actor.CC2DMotor.FaceDirectionChangeHandler += CC2DMotor_FaceDirectionChangeHandler;
        }

        private void CC2DMotor_FaceDirectionChangeHandler(int dir)
        {
            /*if (rightHandItemActor != null)
            {
                ApplyTransformation(rightHandItemActor, true);
            }
            if (leftHandItemActor != null)
            {
                ApplyTransformation(leftHandItemActor, false);
            }*/
        }

        private void ApplyTransformation(TwoHandItemEntity target, bool rightHand)
        {
            target.transform.parent = (rightHand) ? equipmentSpawnPointRight : equipmentSpawnPointLeft;
            target.transform.localScale = Vector3.one;
            target.transform.localPosition = equipmentOffset.GetOffsetPos(rightHand, target.Item.ItemId);
            target.transform.localRotation = Quaternion.Euler(0, 0, equipmentOffset.GetOffsetRot(rightHand, target.Item.ItemId));
        }

        private void EquipFinishedLeftHandler()
        {
            actor.AnimationHandler.ResetAnyStateTransitionPriority(1);
            isInProcessOfEquiping = false;
        }

        private void EquipFinishedRightHandler()
        {
            actor.AnimationHandler.ResetAnyStateTransitionPriority(1);
            isInProcessOfEquiping = false;
        }

        public void EquipItem(bool rightHand, ItemEntity toEquipObject, int itemIndex)
        {

            if (!actor.AnimationHandler.CanAquireAnyStateTransitionPriority(1, 1))
                return;

            Debug.Assert(toEquipObject.GetType() == typeof(TwoHandItemEntity));
            TwoHandItemEntity toEquipActor = (TwoHandItemEntity)toEquipObject;

            actor.Animator.SetTrigger("EquipItem");
            actor.AnimationHandler.SetAnyStateTransitionPriority(1, 1);

            ApplyTransformation(toEquipActor, rightHand);
            toEquipActor.TriggerEquiped(actor, rightHand);
            if (rightHand)
            {
                rightHandItemActor = toEquipActor;
                rightHandItemIndex = itemIndex;
                actor.AnimationHandler.StartListenToAnimationEnd("Equip_Item_Anim", new AnimationHandler.AnimationEvent(EquipFinishedRightHandler));
            }
            else
            {
                leftHandItemActor = toEquipActor;
                leftHandItemIndex = itemIndex;
                actor.AnimationHandler.StartListenToAnimationEnd("Equip_Item_Anim", new AnimationHandler.AnimationEvent(EquipFinishedLeftHandler));
            }
            isInProcessOfEquiping = true;
        }

        public void EquipNextItem(bool rightHand)
        {
            if (actor.CompareTag("Player") && actor.TwoHandInventory.FilledSlotCount == 1)
                return;

            ItemEntity itemActor = rightHand ? rightHandItemActor : leftHandItemActor;
            int itemIndex = rightHand ? rightHandItemIndex : leftHandItemIndex;

            int nxtIndex = actor.TwoHandInventory.GetNextNotEmptyStack(itemIndex);
            if (nxtIndex != -1 && (itemIndex != nxtIndex || itemActor == null))
            {                              
                UnequipItem(rightHand);
                EquipItem(rightHand, actor.TwoHandInventory.GetObjectOfItem(nxtIndex), nxtIndex);
                if (rightHand)
                {
                    HUDManager.hudManager.EquipRight(actor.TwoHandInventory.GetObjectOfItem(nxtIndex).Item);
                }
                else
                {
                    HUDManager.hudManager.EquipLeft(actor.TwoHandInventory.GetObjectOfItem(nxtIndex).Item);
                }
            }
        }

        public void DepleteEquipedItem(bool rightHand, int count)
        {
            TwoHandItemEntity itemActor = rightHand ? rightHandItemActor : leftHandItemActor;
            if (itemActor == null)
                return;

            int itemIndex = rightHand ? rightHandItemIndex : leftHandItemIndex;

            actor.TwoHandInventory.TrashItemFromStack(itemIndex, 1, false);

            if (actor.TwoHandInventory.GetTopItemOfStack(itemIndex) == null)
            {
                if (rightHand)
                {
                    rightHandItemActor = null;
                    Debug.Log("empty");
                    HUDManager.hudManager.EmptyRight();
                }
                else
                    leftHandItemActor = null;
                EquipNextItem(rightHand);
            }
            else
            {
                EquipItem(true, actor.TwoHandInventory.GetObjectOfItem(itemIndex), itemIndex);
            }
        }

        public void DropEquipedItem(bool rightHand)
        {
            TwoHandItemEntity itemActor = rightHand ? rightHandItemActor : leftHandItemActor;
            if (itemActor == null)
                return;

            int itemIndex = rightHand ? rightHandItemIndex : leftHandItemIndex;

            actor.TwoHandInventory.DropFromInventory(rightHandItemIndex);
            itemActor.TriggerDrop();

            if (actor.TwoHandInventory.GetTopItemOfStack(itemIndex) == null)
            {
                if (rightHand)
                    rightHandItemActor = null;
                else
                    leftHandItemActor = null;
                EquipNextItem(rightHand);
            }
            else
            {
                EquipItem(true, actor.TwoHandInventory.GetObjectOfItem(itemIndex), itemIndex);
            }
        }

        public void UnequipItem(bool rightHand)
        {
            TwoHandItemEntity itemActor = rightHand ? rightHandItemActor : leftHandItemActor;

            if (itemActor == null)
            {
                return;
            }

            int itemIndex = rightHand ? rightHandItemIndex : leftHandItemIndex;
            itemActor.TriggerUnequiped();
            actor.TwoHandInventory.PoolCopyOfItem(itemActor);

            if (rightHand)
            {
                rightHandItemActor = null;
            }
            else
                leftHandItemActor = null;
        }

        public bool TryPickMeUp(ItemEntity itemActor)
        {
            if (itemInProccessOfPicking != null || isInProcessOfEquiping)
                return false;

            if (typeof(TwoHandItemEntity) != itemActor.GetType())
                return false;

            int freePlace;
            if (leftHandItemActor == null && actor.TwoHandInventory.CouldAddItemLeft(itemActor.Item))
                freePlace = 0;
            else if (rightHandItemActor == null && actor.TwoHandInventory.CouldAddItemRight(itemActor.Item))
                freePlace = 1;
            else
            {
                freePlace = actor.TwoHandInventory.GetPreferedInventoryIndexForAdding(itemActor.Item);
                if (freePlace == -1)
                    return false;
            }

            if (freePlace == -1 || !actor.AnimationHandler.CanAquireAnyStateTransitionPriority(0, 2))
                return false;

            actor.AnimationHandler.SetAnyStateTransitionPriority(0, 2);
            actor.SetBlockAllInput(true);            
            actor.CC2DMotor.FreezeAndResetMovement(true);
            actor.Animator.SetTrigger("PickUp");
            actor.AnimationHandler.StartListenToAnimationEvent("PickUpReachedItem", new AnimationHandler.AnimationEvent(PickUpReachedItemHandler));
            actor.AnimationHandler.StartListenToAnimationEnd("Item_Pick_up_Anim", new AnimationHandler.AnimationEvent(PickUpFinishedHandler));
            itemInProccessOfPicking = (TwoHandItemEntity)itemActor;
            targetedHand = freePlace;
            UnequipItem(freePlace == 1);
            return true;
        }

        private void PickUpReachedItemHandler()
        {
            ApplyTransformation(itemInProccessOfPicking, targetedHand == 1);
            actor.SetBlockAllInput(false);
            actor.CC2DMotor.FreezeAndResetMovement(false);
            actor.SetBlockAllNonMovement(true);
            itemInProccessOfPicking.TriggerPickUp(actor);
        }

        private void PickUpFinishedHandler()
        {
            actor.AnimationHandler.ResetAnyStateTransitionPriority(0);
            if (targetedHand == 0)
            {
                leftHandItemIndex = actor.TwoHandInventory.AddItem(itemInProccessOfPicking.Item, targetedHand);
                itemInProccessOfPicking.gameObject.SetActive(true);

                itemInProccessOfPicking.TriggerPickUp(actor);
                itemInProccessOfPicking.TriggerEquiped(actor, false);
                leftHandItemActor = itemInProccessOfPicking;

               // HUDManager.hudManager.EquipLeft(itemInProccessOfPicking.Item);
               //Not possible -> binds system to UI (has to be the other way around)

            }
            else
            {
                rightHandItemIndex = actor.TwoHandInventory.AddItem(itemInProccessOfPicking.Item, targetedHand);
                itemInProccessOfPicking.gameObject.SetActive(true);

                itemInProccessOfPicking.TriggerPickUp(actor);
                itemInProccessOfPicking.TriggerEquiped(actor, true);
                rightHandItemActor = itemInProccessOfPicking;

                //HUDManager.hudManager.EquipRight(itemInProccessOfPicking.Item);

            }
            actor.SetBlockAllNonMovement(false);
            itemInProccessOfPicking = null;
        }
    }
}
