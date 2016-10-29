using UnityEngine;
using CC2D;
using Combat;
using ItemHandler;

namespace Actors
{
    public class PlayerActor : BasicEntityWithEquipmentActor
    {
        [SerializeField]
        HumanInput humanInput;
        [SerializeField]
        InteractiveInputUIMarker interactiveInputUIMarker;

        PlayerInteractiveInputHandler playerInteractiveInputHandler;

        #region public
        public HumanInput HumanInput { get { return humanInput; } }
        public PlayerInteractiveInputHandler PlayerInteractiveInputHandler { get { return playerInteractiveInputHandler; } }
        public InteractiveInputUIMarker InteractiveInputUIMarker { get { return interactiveInputUIMarker; } }
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
