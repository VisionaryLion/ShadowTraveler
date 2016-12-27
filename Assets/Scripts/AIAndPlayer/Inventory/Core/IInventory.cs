using UnityEngine;
using Entities;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ItemHandler
{
    public abstract class IInventory : MonoBehaviour, IEnumerable<List<IItem>>
    {

        public abstract int TotalSlotCount { get; }
        public abstract int FilledSlotCount { get; }
        public abstract int FreeSlotCount { get; }

        public abstract IItem GetItem(int stackIndex, int itemIndex);
        public abstract IItem GetTopItemOfStack(int stackIndex);
        public abstract List<IItem> GetStack(int stackIndex);
        public abstract int GetNextNotEmptyStack(int stackIndex);
        public abstract ItemEntity GetObjectOfItem(int stackIndex);

        public abstract bool TryMoveItem(int fromStack, int toStack);
        public abstract bool TryMoveItem(int fromStack, int fromItemIndex, int toStack);
        public abstract bool CouldAddItem(IItem item);
        public abstract int ItemAddingPreferability(IItem item); // How good would it be to add the item to this inventory. -1 means not addable.

        public abstract int AddItem(ItemEntity obj);
        public abstract int AddItem(IItem item);
        public abstract int AddItemToEmptyStack(ItemEntity obj);

        public abstract bool ContainsItem(int id);
        public abstract int FindItemWithId(int id);

        public abstract ItemEntity DropFromInventory(int stackIndex, bool forced = false, bool silent = false);
        public abstract ItemEntity DropFromInventory(int stackIndex, int itemIndex, bool forced = false, bool silent = false);
        public abstract bool TrashItemFromStack(int stackIndex, int count = 1, bool forced = false);
        public abstract bool TrashItemAt(int stackIndex, int itemIndex, bool forced = false);

        public abstract void PoolCopyOfItem(ItemEntity itemInstance);
        public abstract void DeletePoolOfItem(int itemId);

        public delegate void OnInventoryChanged(IInventory inventory);
        public event OnInventoryChanged InventoryChangeHandler;

        public void InvokeOnInventoryChanged(IInventory inv)
        {
            if (InventoryChangeHandler != null)
                InventoryChangeHandler.Invoke(inv);
        }

        public abstract IEnumerator<List<IItem>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
