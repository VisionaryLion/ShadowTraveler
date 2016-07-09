using UnityEngine;
using System.Collections;
using System;

namespace Inventory
{
    [Serializable]
    public class StaticItem : IItem, ICloneable
    {
        [SerializeField]
        ItemData dataSrc;
        [SerializeField]
        bool canBePickedUp = true;
        [SerializeField]
        bool canBeDropped = true;

        int stackTop = 1;

        public StaticItem(ItemData dataSrc, bool canBePickedUp, bool canBeDropped, int stackTop = 1)
        {
            this.dataSrc = dataSrc;
            this.canBePickedUp = canBePickedUp;
            this.canBeDropped = canBeDropped;
            this.stackTop = stackTop;
        }

        #region Getter, Setter
        public override string Description
        {
            get
            {
                return dataSrc.description;
            }
        }

        public override Sprite Icon
        {
            get
            {
                return dataSrc.displaySprite;
            }
        }

        public override bool IsConsumable
        {
            get
            {
                return dataSrc.isConsumable;
            }
        }

        public override bool IsEquipment
        {
            get
            {
                return dataSrc.isEquipment;
            }
        }

        public override bool IsStackable
        {
            get
            {
                return dataSrc.isStackable;
            }
        }

        public override int StackLimit
        {
            get
            {
                return dataSrc.stackLimit;
            }
        }

        public override string Title
        {
            get
            {
                return dataSrc.title;
            }
        }

        public override string Tooltipp
        {
            get
            {
                return dataSrc.tooltips;
            }
        }

        public override int StackTop
        {
            get
            {
                return stackTop;
            }
            set
            {
                stackTop = value;
            }
        }

        public override GameObject ItemPrefab
        {
            get
            {
                return dataSrc.itemPrefab;
            }
        }

        public override bool ShouldPool
        {
            get
            {
                return dataSrc.poolLimit > 0;
            }
        }

        public override int PoolLimit
        {
            get
            {
                return dataSrc.poolLimit;
            }
        }

        public override int ItemId
        {
            get
            {
                return dataSrc.itemID;
            }
        }
        #endregion

        #region Validators
        public override bool CanBeDropped()
        {
            return canBePickedUp;
        }

        public override bool CanBePickedUp()
        {
            return canBeDropped;
        }
        #endregion

        #region Events
        public override bool CanBeStackedWith(IItem other)
        {
            if (!IsStackable || (IsStackable && other.ItemId != ItemId))
                return false;
            return true;
        }

        public override void OnPickedUp(Inventory inv)
        {
            
        }

        public override void OnDropped(Inventory inv, GameObject newItem)
        {
            
        }

        public override void OnTrashed(Inventory inv)
        {
            
        }
        #endregion

        public override bool Equals(IItem other)
        {
            return other.ItemId == ItemId;
        }

        public override string GetStackTopString()
        {
            return (stackTop == 1) ? "" : stackTop.ToString();
        }

        public object Clone()
        {
           return base.MemberwiseClone();
        }
    }
}
