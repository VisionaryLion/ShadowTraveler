/*
 * Inventory script. Add to an empty and parent it to the player.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Inventory
{
    public class Inventory : IInventory
    {
        [SerializeField]
        int inventorySize;//Number of distinct elements the inventory can support 
        [SerializeField]
        InventoryUI inventoryUI;

        public bool IsInventoryFull { get { return inventorySize == filledSlotCount; } }
        public int InventorySize { get { return inventorySize; } }

        IItem[] inventoryCache; // Each index represents a unique tile
        Dictionary<int, Stack<GameObject>> pooledItems;
        int filledSlotCount = 0;

        void Awake()
        {
            //Assign Inventory display component to the reference
            inventoryCache = new IItem[inventorySize];
            pooledItems = new Dictionary<int, Stack<GameObject>>(inventorySize);
        }


        //Will try to add the item as slot saving as possible.
        public override bool AddItem(IItem item, GameObject itemInstance) //TODO: Add system of return codes
        {
            if (IsInventoryFull)
                return false;

            if (item.IsStackable && item.StackLimit > item.StackTop)
            {
                for (int iSlot = 0; iSlot < inventorySize; iSlot++)
                {
                    if (inventoryCache[iSlot] == null)
                        continue;
                    AddItemToStack(item, inventoryCache[iSlot]);
                    if (item.StackTop <= 0)
                    {
                        ManageItemGameObject(item, itemInstance);
                        item.OnPickedUp(this);
                        inventoryUI.UpdateUI();
                        return true;
                    }
                }
                return AddItemToEmptyTile(item, itemInstance); //Add the remainder
            }
            else
            {
                return AddItemToEmptyTile(item, itemInstance);
            }
        }

        //Adds an item to the next open slot.
        public override bool AddItemToEmptyTile(IItem item, GameObject itemInstance)
        {
            if (IsInventoryFull)
                return false;

            for (int iSlot = 0; iSlot < inventorySize; iSlot++)
            {
                if (inventoryCache[iSlot] == null)
                {
                    inventoryCache[iSlot] = item;
                    ManageItemGameObject(item, itemInstance);
                    item.OnPickedUp(this);
                    filledSlotCount++;
                    inventoryUI.UpdateUI();
                    return true;
                }
            }
            return false; //should never happen!
        }

        //Find and delete from cache
        public void TrashItemAt(int index)
        {
            if (inventoryCache[index] == null)
                return;

            if (inventoryCache[index].ShouldPool)
            {
                //Should the pool be deleted?
                bool deltePool = true;
                for (int iSlot = 0; iSlot < index; iSlot++)
                {
                    if (inventoryCache[iSlot].Equals(inventoryCache[index]))
                    {
                        deltePool = false;
                        break;
                    }
                }
                for (int iSlot = index + 1; iSlot < inventorySize; iSlot++)
                {
                    if (inventoryCache[iSlot].Equals(inventoryCache[index]))
                    {
                        deltePool = false;
                        break;
                    }
                }
                if (deltePool)
                    DeletePool(inventoryCache[index]);
            }
            inventoryCache[index] = null;
            filledSlotCount--;
            inventoryUI.UpdateUI();
        }

        //Drop an item from inventory
        public void DropFromInventory(int index)
        {
            if (inventoryCache[index] == null)
                return;
            IItem item = inventoryCache[index];
            GameObject newItem;
            if (item.ShouldPool)
            {
                newItem = DepoolItemGameObject(item);
                newItem.SetActive(true);
            }
            else
                newItem = Instantiate(item.ItemPrefab);
            item.OnDropped(this, newItem);
            inventoryUI.UpdateUI();
        }

        public override bool TryMoveItem(int from, int to)
        {
            if (inventoryCache[to] == null)
            {
                inventoryCache[to] = inventoryCache[from];
                inventoryCache[from] = null;
                inventoryUI.UpdateUI();
                return true;
            }
            else if (inventoryCache[to].CanBeStackedWith(inventoryCache[from]))
            {
                AddItemToStack(inventoryCache[from], inventoryCache[to]);
                //if (inventoryCache[from].StackTop <= 0)
                //    return true;
                inventoryUI.UpdateUI();
                return true; // ambiguous will return true, even if not the whole stack could be moved.
            }
            else
                return false;
        }

        void AddItemToStack(IItem item, IItem target)
        {
            if (!item.CanBeStackedWith(target))
                return;
            int freeSpace = target.StackLimit - target.StackTop;
            if (freeSpace <= 0)
                return;
            int itemsToAdd = Mathf.Min(freeSpace, item.StackTop);
            target.StackTop += itemsToAdd;
            item.StackTop -= itemsToAdd;
        }

        void ManageItemGameObject(IItem item, GameObject itemInstance)
        {
            if (itemInstance == null)
                return;

            if (!item.ShouldPool)
            {
                Destroy(itemInstance);
                return;
            }

            Stack<GameObject> pooledObjs;
            if (pooledItems.TryGetValue(item.ItemId, out pooledObjs))
            {
                if (pooledObjs.Count >= item.PoolLimit)
                {
                    Destroy(itemInstance);
                    return;
                }
                itemInstance.SetActive(false);
                pooledObjs.Push(itemInstance);
                return;
            }
            pooledObjs = new Stack<GameObject>(1);
            pooledObjs.Push(itemInstance);
            pooledItems.Add(item.ItemId, pooledObjs);
            itemInstance.SetActive(false);
        }

        void DeletePool(IItem item)
        {
            pooledItems.Remove(item.ItemId);
        }

        GameObject DepoolItemGameObject(IItem item)
        {
            Stack<GameObject> pooledObjs;
            if (pooledItems.TryGetValue(item.ItemId, out pooledObjs))
            {
                return pooledObjs.Pop();
            }
            return Instantiate(item.ItemPrefab);
        }

        public override IItem GetItem(int index)
        {
            return inventoryCache[index];
        }
    }
}