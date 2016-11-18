using UnityEngine;
using System.Collections;
using System;

namespace ItemHandler
{
    [Serializable]
    public class StaticItem : IItem, ICloneable
    {
        [SerializeField]
        protected ItemData dataSrc;
        [SerializeField]
        protected bool canBePickedUp = true;
        [SerializeField]
        protected bool canBeDropped = true;
        [SerializeField]
        protected bool canBeTrashed = true;

        public StaticItem(ItemData dataSrc, bool canBePickedUp, bool canBeDropped)
        {
            this.dataSrc = dataSrc;
            this.canBePickedUp = canBePickedUp;
            this.canBeDropped = canBeDropped;
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
                return dataSrc.itemName;
            }
        }

        public override string Tooltipp
        {
            get
            {
                return dataSrc.tooltips;
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

        public override ItemData DataSource
        {
            get
            {
                return dataSrc;
            }
        }

        #endregion

        #region Validators
        public override bool CanBeDropped(IInventory inv)
        {
            return canBeDropped;
        }

        public override bool CanBePickedUp(IInventory inv)
        {
            if (dataSrc.canHoldOnlyOne && inv.ContainsItem(dataSrc.itemID))
                return false;
            return canBePickedUp;
            
        }

        public override bool CanBeTrashed(IInventory inv)
        {
            return canBeTrashed;
        }
        #endregion

        #region Events
        #endregion

        public override bool Equals(IItem other)
        {
            if (other == null)
                return false;
            return other.ItemId == ItemId;
        }

        public virtual object Clone()
        {
           return base.MemberwiseClone();
        }
    }
}
