using UnityEngine;
using System;

namespace Inventory
{
    //Fake interface, to please Unity
    [Serializable]
    public abstract class IItem : IEquatable<IItem>
    {
        //Validators
        public abstract bool CanBePickedUp();
        public abstract bool CanBeDropped();
        public abstract bool CanBeStackedWith(IItem other);

        //Events
        public abstract void OnPickedUp(Inventory inv);
        public abstract void OnDropped(Inventory inv, GameObject newItem);
        public abstract void OnTrashed(Inventory inv);
        public abstract bool Equals(IItem other);

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
    }
}
