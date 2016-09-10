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
                actor.MultiSlotsInventory.AddGameObjectCopyOfItem(0, currentEquipedItemLeft, currentEquipedGameObjectLeft);
            if (currentEquipedItemLeft != null)
                currentEquipedGameObjectLeft.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);

            currentEquipedGameObjectLeft = gameObject;
            currentEquipedItemLeft = item;
            currentEquipedGameObjectLeft.transform.position = equipmentSpawnPointLeft.position;
            currentEquipedGameObjectLeft.transform.rotation = equipmentSpawnPointLeft.rotation;
            currentEquipedGameObjectLeft.transform.parent = equipmentSpawnPointLeft;

            actor.Animator.SetTrigger("EquipItem");
            actor.AnimationHandler.SetAnyStateTransitionPriority(1, 1);
            actor.AnimationHandler.StartListenToAnimationEnd("Equip_Item_Anim", new AnimationHandler.AnimationEvent(EquipFinishedLeftHandler));
        }

        public void EquipItemRight(IItem item, GameObject gameObject)
        {
            if (!actor.AnimationHandler.CanAquireAnyStateTransitionPriority(1, 1))
                return;
            if (currentEquipedItemRight != null)
                actor.MultiSlotsInventory.AddGameObjectCopyOfItem(0, currentEquipedItemRight, currentEquipedGameObjectRight);
            if (currentEquipedItemRight != null)
                currentEquipedGameObjectRight.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);

            currentEquipedGameObjectRight = gameObject;
            currentEquipedItemRight = item;
            currentEquipedGameObjectRight.transform.position = equipmentSpawnPointRight.position;
            currentEquipedGameObjectRight.transform.rotation = equipmentSpawnPointRight.rotation;
            currentEquipedGameObjectRight.transform.parent = equipmentSpawnPointRight;

            actor.Animator.SetTrigger("EquipItem");
            actor.AnimationHandler.SetAnyStateTransitionPriority(1, 1);
            actor.AnimationHandler.StartListenToAnimationEnd("Equip_Item_Anim", new AnimationHandler.AnimationEvent(EquipFinishedRightHandler));
        }

        public Transform GetFreeHand()
        {
            if (currentEquipedGameObjectLeft == null)
                return equipmentSpawnPointLeft;
            if (currentEquipedGameObjectRight == null)
                return equipmentSpawnPointRight;
            return equipmentSpawnPointLeft;
        }
    }
}
