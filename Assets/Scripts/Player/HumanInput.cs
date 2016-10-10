using UnityEngine;
using Actors;
using System;
using ItemHandler;

namespace CC2D
{
    public class HumanInput : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [AssignActorAutomaticly]
        PlayerActor actor;

        [SerializeField]
        [Tooltip("Max time a jump will be buffered.")]
        float maxJumpExecutionDelay = 0.5f;

        [SerializeField]
        EquipmentButtonBinding[] binds; //according to keyboard numbers

        MovementInput bufferedInput;
        bool allowMovementInput = true;
        bool allowEquipmentInput = true;

        void Awake()
        {
            bufferedInput = new MovementInput();
            actor.CC2DMotor.CurrentMovementInput = bufferedInput;
        }

        void Update()
        {
            if (allowMovementInput)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    bufferedInput.AddEvent(new JumpEvent(maxJumpExecutionDelay));
                }
                else if (Input.GetButtonDown("Crouch"))
                {
                    bufferedInput.AddEvent(new CrouchEvent());

                    bufferedInput.toggleCrouch = !bufferedInput.toggleCrouch;
                }
            }

            if (allowEquipmentInput)
            {
                /*foreach (EquipmentButtonBinding b in binds)
                {
                    if (currentBind == b)
                        continue;

                    if (Input.GetKeyDown(b.key) && actor.MultiSlotsInventory.ContainsItem(b.equipment.itemID) && !actor.PlayerLimitationHandler.AreAnimationTriggerLocked())
                    {
                        if (currentEquipedItem != null)
                        {
                            actor.MultiSlotsInventory.AddGameObjectCopyOfItem(currentEquipedItem, currentPhysicalEquipment);
                        }
                        if (currentPhysicalEquipment != null)
                            currentPhysicalEquipment.SendMessage("OnUnequiped", SendMessageOptions.DontRequireReceiver);
                        int itemIndex = actor.MultiSlotsInventory.FindItem(b.equipment.itemID);
                        currentEquipedItem = actor.MultiSlotsInventory.GetItem(itemIndex);
                        currentPhysicalEquipment = actor.MultiSlotsInventory.EquipItem(itemIndex);
                        currentPhysicalEquipment.transform.position = equipSpawnPoint.position;
                        currentPhysicalEquipment.transform.rotation = equipSpawnPoint.rotation;
                        currentPhysicalEquipment.transform.parent = equipSpawnPoint;
                        currentBind = b;

                        actor.PlayerAnimationUpperBodyEnd.EquipFinishedHandler += PlayerAnimationEventGrabberFront_EquipFinishedHandler;
                        //actor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.BlockEquipmentUse);
                        actor.CC2DMotor.frontAnimator.SetTrigger("EquipItem");
                    }
                }*/
            }
        }

        void FixedUpdate()
        {
            if (!allowMovementInput)
                return;

            bufferedInput.horizontalRaw = Input.GetAxisRaw("Horizontal");
            bufferedInput.verticalRaw = Input.GetAxisRaw("Vertical");

            bufferedInput.horizontal = Input.GetAxis("Horizontal");
            bufferedInput.vertical = Input.GetAxis("Vertical");
        }

        public void SetAllowAllInput(bool enabled)
        {
            SetAllowEquipmentInput(enabled);
            SetAllowMovementInput(enabled);
        }

        public void SetAllowMovementInput(bool enabled)
        {
            allowMovementInput = enabled;
            if (!allowMovementInput)
                ResetPlayerMovementInput();
        }

        public void SetAllowEquipmentInput(bool enabled)
        {
            allowEquipmentInput = enabled;
        }

        public void ResetPlayerMovementInput()
        {
            bufferedInput.horizontal = 0;
            bufferedInput.horizontalRaw = 0;
            bufferedInput.vertical = 0;
            bufferedInput.verticalRaw = 0;
        }

        [Serializable]
        public class EquipmentButtonBinding
        {
            public KeyCode key;
            public ItemData equipment;
        }
    }
}
