using UnityEngine;
using System;

namespace ItemHandler
{
    //Fake interface, to please Unity
    public abstract class IItem : IEquatable<IItem>
    {
        //event delegates
        public delegate void OnItemDrop(IInventory inv);
        public delegate void OnItemTrash(IInventory inv);
        public delegate void OnItemPickUp(IInventory inv);

        //Validators
        public abstract bool CanBePickedUp(IInventory inv);
        public abstract bool CanBeTrashed(IInventory inv);
        public abstract bool CanBeDropped(IInventory inv);
        public abstract bool CanBeStackedWith(IItem other);

        //Events
        public virtual void OnPickedUp(IInventory inv)
        {
            if (ItemPickUpHandler != null)
                ItemPickUpHandler.Invoke(inv);
        }

        public virtual void OnDropped(IInventory inv, GameObject newItem)
        {
            if (ItemDropHandler != null)
                ItemDropHandler.Invoke(inv);
        }

        public virtual void OnTrashed(IInventory inv)
        {
            if (ItemTrashHandler != null)
                ItemTrashHandler.Invoke(inv);
        }

        public abstract bool Equals(IItem other);

        public event OnItemDrop ItemDropHandler;
        public event OnItemTrash ItemTrashHandler;
        public event OnItemPickUp ItemPickUpHandler;

        //data access
        public abstract int ItemId { get; }
        public abstract Sprite Icon { get; }
        public abstract GameObject ItemPrefab { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }
        public abstract string Tooltipp { get; }
        public abstract bool IsEquipment { get; }
        public abstract bool IsConsumable { get; }
        public abstract bool IsStackable { get; }
        public abstract bool ShouldPool { get; }
        public abstract int StackLimit { get; }
        public abstract int StackTop { get; set; }
        public abstract int PoolLimit { get; } // 0 = no pooling

        public abstract string GetStackTopString();

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(IItem))
                return false;

            return ((IItem)obj).ItemId == ItemId;
        }
    }
}
