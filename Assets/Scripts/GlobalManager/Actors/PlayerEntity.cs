using UnityEngine;
using CC2D;
using Combat;
using ItemHandler;

namespace Entities
{
    public class PlayerEntity : ActingEquipmentEntity
    {
        [SerializeField]
        HumanInput humanInput;
        [SerializeField]
        InteractiveInputUIMarker interactiveInputUIMarker;
        [SerializeField]
        HumanMovementEntity humanMovementActor;

        PlayerInteractiveInputHandler playerInteractiveInputHandler;

        #region public
        public HumanInput HumanInput { get { return humanInput; } }
        public PlayerInteractiveInputHandler PlayerInteractiveInputHandler { get { return playerInteractiveInputHandler; } }
        public InteractiveInputUIMarker InteractiveInputUIMarker { get { return interactiveInputUIMarker; } }

        public CharacterController2D CharacterController2D { get { return movementEntity.CharacterController2D; } }
        public CC2DMotor CC2DMotor { get { return movementEntity.CC2DMotor; } }
        public Rigidbody2D Rigidbody2D { get { return movementEntity.Rigidbody2D; } }
        public BoxCollider2D BoxCollider2D { get { return movementEntity.BoxCollider2D; } }
        #endregion

        protected override void InitInteractiveInputHandler()
        {
            playerInteractiveInputHandler = new PlayerInteractiveInputHandler(this);
            interactiveInputHandler = playerInteractiveInputHandler;
        }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();
            
            //Load components
            humanInput = GetComponentInChildren<HumanInput>();
            interactiveInputUIMarker = GetComponentInChildren<InteractiveInputUIMarker>();
            humanMovementActor = LoadComponent<HumanMovementEntity>(humanMovementActor);
            movementEntity = humanMovementActor;

            //Setup some script vars automatically.
            this.tag = "Player"; //Built-in-Tag can't go wrong.

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminder("Controller", "Animator"));
        }
#endif

        public override void SetBlockAllInput(bool blockInput)
        {
            Debug.Log("Block all player Input was set to "+blockInput);
            humanInput.SetAllowAllInput(!blockInput);
            playerInteractiveInputHandler.blockAllInput = blockInput;
        }

        public override void SetBlockAllNonMovement(bool blockInput) {
            Debug.Log("Block all non movement player Input was set to " + blockInput);
            humanInput.SetAllowEquipmentInput(!blockInput);
            playerInteractiveInputHandler.blockAllInput = blockInput;
        }
    }
}
