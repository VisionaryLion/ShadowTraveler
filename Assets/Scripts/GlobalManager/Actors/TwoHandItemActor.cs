using UnityEngine;
using System.Collections;
using ItemHandler;

namespace Actors
{
    public class TwoHandItemActor : ItemActor
    {
        #region public
        public delegate void OnEquiped(BasicEntityEquipmentActor equiper);
        public delegate void OnUnequiped();
        public delegate void OnPickedUp(BasicEntityEquipmentActor equiper);
        public delegate void OnDroped();

        public event OnEquiped EquipedHandler;
        public event OnUnequiped UnequipedHandler;
        public event OnPickedUp PickUpHandler;
        public event OnDroped DropedHandler;
        #endregion

        bool equipedRight;

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();
        }
#endif

        public void TriggerEquiped(BasicEntityEquipmentActor equiper, bool rightHand)
        {
            equipedRight = rightHand;

            if (EquipedHandler != null)
                EquipedHandler.Invoke(equiper);
        }

        public void TriggerUnequiped()
        {
            if (UnequipedHandler != null)
                UnequipedHandler.Invoke();
        }

        public void TriggerPickUp(BasicEntityEquipmentActor equiper)
        {
            if (PickUpHandler != null)
                PickUpHandler.Invoke(equiper);
        }

        public void TriggerDrop()
        {
            if (DropedHandler != null)
                DropedHandler.Invoke();
        }

        public bool EquipedWithRightHand { get { return equipedRight; } }
    }
}
