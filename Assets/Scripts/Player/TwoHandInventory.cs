using UnityEngine;
using System.Collections;
using Entities;
using System;
using System.Collections.Generic;

namespace ItemHandler
{
    public class TwoHandInventory : IInventory
    {
        [SerializeField]
        Inventory leftInventory;
        [SerializeField]
        Inventory rightInventory;

        public override int FilledSlotCount
        {
            get
            {
                return leftInventory.FilledSlotCount + rightInventory.FilledSlotCount;
            }
        }

        public override int FreeSlotCount
        {
            get
            {
                return leftInventory.FreeSlotCount + rightInventory.FreeSlotCount;
            }
        }

        public int FreeSlotCountRight
        {
            get
            {
                return rightInventory.FreeSlotCount;
            }
        }

        public int FreeSlotCountLeft
        {
            get
            {
                return leftInventory.FreeSlotCount;
            }
        }

        public override int TotalSlotCount
        {
            get
            {
                return leftInventory.TotalSlotCount + rightInventory.TotalSlotCount;
            }
        }

        public override int AddItem(ItemEntity obj)
        {
            int preferabillityLeft = leftInventory.ItemAddingPreferability(obj.Item);
            int preferabillityRight = rightInventory.ItemAddingPreferability(obj.Item);

            if (preferabillityLeft >= preferabillityRight)
            {
                if (preferabillityLeft == -1)
                    return -1;
                return LocalToWorldIndex(0, leftInventory.AddItem(obj));
            }
            else
            {
                return LocalToWorldIndex(1, rightInventory.AddItem(obj));
            }
        }

        public override int AddItem(IItem item)
        {
            int preferabillityLeft = leftInventory.ItemAddingPreferability(item);
            int preferabillityRight = rightInventory.ItemAddingPreferability(item);

            if (preferabillityLeft >= preferabillityRight)
            {
                if (preferabillityLeft == -1)
                    return -1;
                return LocalToWorldIndex(0, leftInventory.AddItem(item));
            }
            else
            {
                return LocalToWorldIndex(1, rightInventory.AddItem(item));
            }
        }

        public int AddItem(IItem item, int inventoryIndex)
        {
            if (inventoryIndex == 0)
            {
                return LocalToWorldIndex(0, leftInventory.AddItem(item));
            }
            else
            {
                return LocalToWorldIndex(1, rightInventory.AddItem(item));
            }
        }

        public override int AddItemToEmptyStack(ItemEntity obj)
        {
            if (leftInventory.FreeSlotCount >= rightInventory.FreeSlotCount)
            {
                if (leftInventory.FreeSlotCount <= 0)
                    return -1;
                return LocalToWorldIndex(0, leftInventory.AddItemToEmptyStack(obj));
            }
            else
            {
                return LocalToWorldIndex(1, rightInventory.AddItemToEmptyStack(obj));
            }
        }

        public override bool ContainsItem(int id)
        {
            return leftInventory.ContainsItem(id) || rightInventory.ContainsItem(id);
        }

        public override bool CouldAddItem(IItem item)
        {
            return leftInventory.CouldAddItem(item) || rightInventory.CouldAddItem(item);
        }

        public bool CouldAddItemLeft(IItem item)
        {
            return leftInventory.CouldAddItem(item);
        }

        public bool CouldAddItemRight(IItem item)
        {
            return rightInventory.CouldAddItem(item);
        }

        public override void DeletePoolOfItem(int itemId)
        {
            leftInventory.DeletePoolOfItem(itemId);
            rightInventory.DeletePoolOfItem(itemId);
        }

        public override ItemEntity DropFromInventory(int stackIndex, bool forced = false, bool silent = false)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).DropFromInventory(localStackIndex, 0, forced, silent);
        }

        public override ItemEntity DropFromInventory(int stackIndex, int itemIndex, bool forced = false, bool silent = false)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).DropFromInventory(localStackIndex, itemIndex, forced, silent);
        }

        public override int FindItemWithId(int id)
        {
            int index = leftInventory.FindItemWithId(id);
            if (index == -1)
                index = LocalToWorldIndex(1, rightInventory.FindItemWithId(id));
            else
                index = LocalToWorldIndex(0, index);
            return index;
        }

        public override IEnumerator<List<IItem>> GetEnumerator()
        {
            return new TwoHandInventoryEnumerator(this);
        }

        public override IItem GetItem(int stackIndex, int itemIndex)
        {
            int localItemIndex;
            return GetInventory(stackIndex, out localItemIndex).GetItem(localItemIndex, itemIndex);
        }

        public override List<IItem> GetStack(int stackIndex)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).GetStack(localStackIndex);
        }

        public override int GetNextNotEmptyStack(int stackIndex)
        {
            int localStackIndex;
            return LocalToWorldIndexWithOrg(stackIndex, GetInventory(stackIndex, out localStackIndex).GetNextNotEmptyStack(localStackIndex));
        }

        public override ItemEntity GetObjectOfItem(int stackIndex)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).GetObjectOfItem(localStackIndex);
        }

        public override IItem GetTopItemOfStack(int stackIndex)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).GetTopItemOfStack(localStackIndex);
        }

        public override int ItemAddingPreferability(IItem item)
        {
            return Mathf.Max(leftInventory.ItemAddingPreferability(item), rightInventory.ItemAddingPreferability(item));
        }

        public override void PoolCopyOfItem(ItemEntity itemInstance)
        {
            if (leftInventory.ContainsItem(itemInstance.Item.ItemId))
            {
                leftInventory.PoolCopyOfItem(itemInstance);
            }
            else
            {
                rightInventory.PoolCopyOfItem(itemInstance);
            }
        }

        public override bool TrashItemAt(int stackIndex, int itemIndex, bool forced = false)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).TrashItemAt(localStackIndex, itemIndex, forced);
        }

        public override bool TrashItemFromStack(int stackIndex, int count = 1, bool forced = false)
        {
            int localStackIndex;
            return GetInventory(stackIndex, out localStackIndex).TrashItemFromStack(localStackIndex, count, forced);
        }

        public override bool TryMoveItem(int fromStack, int toStack)
        {
            return TryMoveItem(fromStack, toStack, 0);
        }

        public override bool TryMoveItem(int fromStack, int fromItemIndex, int toStack)
        {
            int stackIndexFrom;
            var invFrom = GetInventory(fromStack, out stackIndexFrom);

            int stackIndexTo;
            var invTo = GetInventory(fromStack, out stackIndexTo);

            if (invFrom == invTo)
            {
                return invFrom.TryMoveItem(stackIndexFrom, stackIndexTo);
            }
            else
            {
                var stackTo = invTo.GetStack(stackIndexTo);
                var itemFrom = invFrom.GetItem(stackIndexFrom, fromItemIndex);

                if (stackTo.Count >= itemFrom.StackLimit)
                    return false;

                ItemEntity itemObjFrom = invFrom.DropFromInventory(stackIndexFrom, fromItemIndex, true, true);
                stackTo.Add(itemObjFrom.Item);
                invTo.PoolCopyOfItem(itemObjFrom);
                return true;
            }
        }

        private int GlobalToLocalItemIndex(int itemIndex)
        {
            if (itemIndex < leftInventory.TotalSlotCount)
                return itemIndex;
            else if (itemIndex < rightInventory.TotalSlotCount)
                return itemIndex - leftInventory.TotalSlotCount;
            else
                return itemIndex - leftInventory.TotalSlotCount - rightInventory.TotalSlotCount;
        }        

        private int LocalToWorldIndexWithOrg(int orgWorldIndex, int localIndex)
        {
            if (orgWorldIndex < leftInventory.TotalSlotCount)
                return localIndex;
            else
                return localIndex + leftInventory.TotalSlotCount;
        }

        private int LocalToWorldIndex(int inventoryIndex, int localIndex)
        {
            if (inventoryIndex == 0)
                return localIndex;
            else
                return localIndex + leftInventory.TotalSlotCount;
        }

        private Inventory GetInventory(int itemIndex, out int localItemIndex)
        {
            if (itemIndex < leftInventory.TotalSlotCount)
            {
                localItemIndex = itemIndex;
                return leftInventory;
            }
            else
            {
                localItemIndex = itemIndex - leftInventory.TotalSlotCount;
                return rightInventory;
            }
        }

        private Inventory GetInventory(int itemIndex)
        {
            if (itemIndex < leftInventory.TotalSlotCount)
            {
                return leftInventory;
            }
            else
            {
                return rightInventory;
            }
        }

        public int GetPreferedInventoryIndexForAdding(IItem item)
         {
            int preferabillityLeft = leftInventory.ItemAddingPreferability(item);
            int preferabillityRight = rightInventory.ItemAddingPreferability(item);

            if (preferabillityLeft >= preferabillityRight)
            {
                if (preferabillityLeft == -1)
                    return -1;
                return 0;
            }
            else
            {
                return 1;
            }
        }

        class TwoHandInventoryEnumerator : IEnumerator<List<IItem>>
        {
            TwoHandInventory source;
            int currentStackIndex;

            public TwoHandInventoryEnumerator(TwoHandInventory source)
            {
                this.source = source;
                this.currentStackIndex = 0;
            }

            public List<IItem> Current
            {
                get
                {
                    return source.GetStack(currentStackIndex);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                source = null;
            }

            public bool MoveNext()
            {
                currentStackIndex++;
                if (currentStackIndex >= source.TotalSlotCount)
                    return false;
                return true;
            }

            public void Reset()
            {
                currentStackIndex = 0;
            }
        }
    }
}
