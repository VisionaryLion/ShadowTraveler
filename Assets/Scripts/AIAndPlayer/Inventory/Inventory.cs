using UnityEngine;
using System.Collections.Generic;
using System;
using Entity;

namespace ItemHandler
{
    public class Inventory : IInventory
    {
        [SerializeField]
        int inventorySize;//Number of distinct elements the inventory can support 
        [SerializeField]
        int initialPerSlotAllocationCount; //Initial size of the item holding list for each slot

        public bool AllSlotsFull { get { return inventorySize == filledSlotCount; } }

        public override int TotalSlotCount
        {
            get
            {
                return inventorySize;
            }
        }

        public override int FilledSlotCount
        {
            get
            {
                return filledSlotCount;
            }
        }

        public override int FreeSlotCount
        {
            get
            {
                return inventorySize - filledSlotCount;
            }
        }

        List<IItem>[] inventoryCache; // Each index represents a unique tile
        Dictionary<int, Stack<ItemEntity>> pooledItems;
        int filledSlotCount = 0;

        void Awake()
        {
            inventoryCache = new List<IItem>[inventorySize];
            for (int iStack = 0; iStack < inventorySize; iStack++)
            {
                inventoryCache[iStack] = new List<IItem>(initialPerSlotAllocationCount);
            }
            pooledItems = new Dictionary<int, Stack<ItemEntity>>(inventorySize);
        }

        public override IItem GetItem(int stackIndex, int itemIndex)
        {
            return inventoryCache[stackIndex][itemIndex];
        }

        public override IItem GetTopItemOfStack(int stackIndex)
        {
            if (inventoryCache[stackIndex].Count == 0)
                return null;
            return inventoryCache[stackIndex][0];
        }

        public override List<IItem> GetStack(int stackIndex)
        {
            return inventoryCache[stackIndex];
        }

        public override int GetNextNotEmptyStack(int stackIndex)
        {
            for (int iStack = stackIndex + 1; iStack < inventoryCache.Length; iStack++)
            {
                if (inventoryCache[iStack].Count != 0)
                    return iStack;
            }
            for (int iStack = 0; iStack <= stackIndex; iStack++)
            {
                if (inventoryCache[iStack].Count != 0)
                    return iStack;
            }
            return -1;
        }

        public override ItemEntity GetObjectOfItem(int stackIndex)
        {
            Debug.Assert(inventoryCache[stackIndex].Count != 0);
            IItem item = inventoryCache[stackIndex][0];

            ItemEntity newItem;
            Stack<ItemEntity> pooledObjs;
            if (item.ShouldPool && pooledItems.TryGetValue(item.ItemId, out pooledObjs))
            {
                ItemEntity actor = pooledObjs.Pop();
                if (pooledObjs.Count == 0)
                    pooledItems.Remove(item.ItemId);
                newItem = actor;
                newItem.gameObject.SetActive(true);
            }
            else
                newItem = Instantiate(item.ItemPrefab).GetComponent<ItemEntity>();

            newItem.Item = item;
            return newItem;
        }

        public override bool TryMoveItem(int fromStack, int toStack)
        {
            return TryMoveItem(fromStack, 0, toStack);
        }

        public override bool TryMoveItem(int fromStack, int fromItemIndex, int toStack)
        {
            var targetStack = inventoryCache[toStack];
            var sourceStack = inventoryCache[fromStack];

            if (targetStack[0].ItemId != sourceStack[fromItemIndex].ItemId || targetStack[0].StackLimit >= targetStack.Count + 1)
                return false;

            targetStack.Add(sourceStack[fromItemIndex]);
            sourceStack.RemoveAt(fromItemIndex);
            return true;
        }

        public override bool CouldAddItem(IItem item)
        {
            if (item.IsStackable)
            {
                List<IItem> stack;
                int emptyStackIndex = -1;
                for (int iStack = 0; iStack < inventoryCache.Length; iStack++)
                {
                    stack = inventoryCache[iStack];

                    if (stack.Count == 0)
                    {
                        emptyStackIndex = iStack;
                        continue;
                    }
                    if (stack[0].ItemId != item.ItemId || item.StackLimit >= stack.Count)
                        continue;

                    return true;
                }

                if (emptyStackIndex != -1)
                    return true;
                else
                    return false;
            }
            else
            {
                if (AllSlotsFull)
                    return false;

                List<IItem> stack;
                for (int iStack = 0; iStack < inventoryCache.Length; iStack++)
                {
                    stack = inventoryCache[iStack];
                    if (stack.Count == 0)
                    {
                        return true;
                    }
                }
                throw new Exception("Inventory isn't full, but we couldn't add an item to an empty slot.");
            }
        }

        public override int AddItem(ItemEntity obj)
        {
            int newStackIndex = AddItem(obj.Item);
            if (newStackIndex == -1)
                return -1;
            PoolCopyOfItem(obj);
            return newStackIndex;
        }

        public override int AddItem(IItem item)
        {
            if (item.IsStackable)
            {
                List<IItem> stack;
                int emptyStackIndex = -1;
                for (int iStack = 0; iStack < inventoryCache.Length; iStack++)
                {
                    stack = inventoryCache[iStack];

                    if (stack.Count == 0)
                    {
                        emptyStackIndex = iStack;
                        continue;
                    }
                    if (stack[0].ItemId != item.ItemId || item.StackLimit >= stack.Count)
                        continue;

                    stack.Add(item);
                    return iStack;
                }

                if (emptyStackIndex != -1)
                {
                    inventoryCache[emptyStackIndex].Add(item);
                    filledSlotCount++;
                    return emptyStackIndex;
                }
                else
                    return -1;
            }
            else
            {
                return AddItemToEmptyStack(item);
            }
        }

        public override int AddItemToEmptyStack(ItemEntity obj)
        {
            Debug.Assert(obj != null);
            int newStackIndex = AddItemToEmptyStack(obj.Item);
            if (newStackIndex == -1)
                return -1;
            PoolCopyOfItem(obj);
            return newStackIndex;
        }

        public override bool ContainsItem(int id)
        {
            return FindItemWithId(id) != -1;
        }

        public override int FindItemWithId(int id)
        {
            for (int iSlot = 0; iSlot < inventorySize; iSlot++)
            {
                if (inventoryCache[iSlot].Count == 0)
                    continue;

                if (inventoryCache[iSlot][0].ItemId == id)
                {
                    return iSlot;
                }
            }
            return -1;
        }

        public override ItemEntity DropFromInventory(int stackIndex, bool forced = false, bool silent = false)
        {
            return DropFromInventory(stackIndex, 0, forced, silent);
        }

        public override ItemEntity DropFromInventory(int stackIndex, int itemIndex, bool forced = false, bool silent = false)
        {
            Debug.Assert(inventoryCache.Length < stackIndex && stackIndex > 0 && inventoryCache[stackIndex].Count > itemIndex && itemIndex > 0);

            if (!forced && !inventoryCache[stackIndex][itemIndex].CanBeDropped(this))
                return null;

            ItemEntity newActor = GetObjectOfItem(stackIndex);

            if (inventoryCache[stackIndex].Count == 1)
            {
                pooledItems.Remove(inventoryCache[stackIndex][itemIndex].ItemId);
                filledSlotCount--;
            }
            inventoryCache[stackIndex].RemoveAt(itemIndex);
            return newActor;
        }

        public override bool TrashItemFromStack(int stackIndex, int count = 1, bool forced = false)
        {
            for (int deletedItems = 0; deletedItems < count; deletedItems++)
            {
                if (!TrashItemAt(stackIndex, 0))
                    return false;
            }
            return true;
        }

        public override bool TrashItemAt(int stackIndex, int itemIndex, bool forced = false)
        {
            Debug.Assert(inventoryCache.Length > stackIndex && stackIndex >= 0 && inventoryCache[stackIndex].Count > itemIndex && itemIndex >= 0);

            if (!forced && !inventoryCache[stackIndex][itemIndex].CanBeTrashed(this))
                return false;

            if (inventoryCache[stackIndex].Count == 1)
            {
                pooledItems.Remove(inventoryCache[stackIndex][itemIndex].ItemId);
                filledSlotCount--;
            }
            inventoryCache[stackIndex].RemoveAt(itemIndex);
            return true;
        }

        public override void PoolCopyOfItem(ItemEntity itemInstance)
        {
            Debug.Assert(itemInstance != null);

            if (!itemInstance.Item.ShouldPool)
            {
                Destroy(itemInstance.gameObject);
                return;
            }

            if (!ContainsItem(itemInstance.Item.ItemId))
            {
                Destroy(itemInstance.gameObject);
                return;
            }

            Stack<ItemEntity> pooledObjs;
            if (pooledItems.TryGetValue(itemInstance.Item.ItemId, out pooledObjs))
            {
                if (pooledObjs.Count >= itemInstance.Item.PoolLimit)
                {
                    Destroy(itemInstance);
                    return;
                }
                itemInstance.gameObject.SetActive(false);
                itemInstance.transform.parent = transform;
                pooledObjs.Push(itemInstance);
                return;
            }
            pooledObjs = new Stack<ItemEntity>(1);
            pooledObjs.Push(itemInstance);
            pooledItems.Add(itemInstance.Item.ItemId, pooledObjs);
            itemInstance.gameObject.SetActive(false);
            itemInstance.transform.parent = transform;
        }

        public override void DeletePoolOfItem(int itemId)
        {
            pooledItems.Remove(itemId);
        }

        public override int ItemAddingPreferability(IItem item)
        {
            if (item.IsStackable)
            {
                List<IItem> stack;
                int emptyStackIndex = -1;
                for (int iStack = 0; iStack < inventoryCache.Length; iStack++)
                {
                    stack = inventoryCache[iStack];

                    if (stack.Count == 0)
                    {
                        emptyStackIndex = iStack;
                        continue;
                    }
                    if (stack[0].ItemId != item.ItemId || item.StackLimit >= stack.Count)
                        continue;

                    return 1000000000;
                }

                if (emptyStackIndex != -1)
                    return FreeSlotCount;
                else
                    return -1;
            }
            else
            {
                if (AllSlotsFull)
                    return -1;

                List<IItem> stack;
                for (int iStack = 0; iStack < inventoryCache.Length; iStack++)
                {
                    stack = inventoryCache[iStack];
                    if (stack.Count == 0)
                    {
                        return FreeSlotCount;
                    }
                }
                throw new Exception("Inventory isn't full, but we couldn't add an item to an empty slot.");
            }
        }

        public override IEnumerator<List<IItem>> GetEnumerator()
        {
            return (IEnumerator<List<IItem>>)inventoryCache.GetEnumerator();
        }

        int AddItemToEmptyStack(IItem item)
        {
            if (AllSlotsFull)
                return -1;

            List<IItem> stack;
            for (int iStack = 0; iStack < inventoryCache.Length; iStack++)
            {
                stack = inventoryCache[iStack];
                if (stack.Count == 0)
                {
                    stack.Add(item);
                    filledSlotCount++;
                    return iStack;
                }
            }
            throw new Exception("Inventory isn't full, but we couldn't add an item to an empty slot.");
        }
    }
}