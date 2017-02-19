using UnityEngine;
using System.Collections;
using ItemHandler;

namespace Entities
{
    public class ItemEntity : Entity
    {
        [SerializeField]
        IItemHolder itemHolder;

        #region public
        public IItem Item { get { return itemHolder.Item; } set { itemHolder.Item = value; } }

        public delegate void OnEquiped(ActingEquipmentEntity equiper);
        public delegate void OnUnequiped();
        public delegate void OnPickedUp(ActingEquipmentEntity equiper);
        public delegate void OnDroped();

        public event OnEquiped EquipedHandler;
        public event OnUnequiped UnequipedHandler;
        public event OnPickedUp PickUpHandler;
        public event OnDroped DropedHandler;
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            itemHolder = LoadComponent<IItemHolder>(itemHolder);
        }
#endif

        public void TriggerEquiped(ActingEquipmentEntity equiper)
        {
            if (EquipedHandler != null)
                EquipedHandler.Invoke(equiper);
        }

        public void TriggerUnequiped()
        {
            if (UnequipedHandler != null)
                UnequipedHandler.Invoke();
        }

        public void TriggerPickUp(ActingEquipmentEntity equiper)
        {
            if (PickUpHandler != null)
                PickUpHandler.Invoke(equiper);
        }

        public void TriggerDrop()
        {
            if (DropedHandler != null)
                DropedHandler.Invoke();
        }
    }
}
