using UnityEngine;
using System.Collections;

namespace ItemHandler
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField]
        Inventory leftInventory;
        [SerializeField]
        Inventory rightInventory;
        [SerializeField]
        Inventory mainInventory;

        public enum InventoryType { Right = 0, Left = 1, Main = 2 }

        public IItem GetItem(InventoryType inventoryType, int itemIndex)
        {
            return GetInv(inventoryType).GetItem(itemIndex);
        }

        public bool TryMoveItem(InventoryType fromInv, int fromItem, InventoryType toInv, int toItem)
        {
            if (fromInv == toInv)
            {
                return GetInv(fromInv).TryMoveItem(fromItem, toItem);
            }
            else
            {
                IItem item = GetInv(fromInv).GetItem(fromItem);
                if (!GetInv(toInv).CouldAddItem(item))
                    return false;
                GameObject physItem = GetInv(fromInv).DropFromInventorySilent(fromItem);
                if (!GetInv(toInv).AddItem(item, physItem))
                {
                    Debug.LogError("Error happened while transferring item. Its now lost!");
                    return false;
                }
                return true;
            }
        }

        public bool AddItem(InventoryType inventoryType, IItem item, GameObject obj)
        {
            return GetInv(inventoryType).AddItem(item, obj);
        }

        public bool AddEquipment(IItem item, GameObject obj)
        {
            int l = (leftInventory.CouldAddItem(item) ? 1 : 0) * ((leftInventory.ContainsItem(item.ItemId) ? 100000000 : 0) + leftInventory.InventoryFreeSpace);
            int r = (rightInventory.CouldAddItem(item) ? 1 : 0) * ((rightInventory.ContainsItem(item.ItemId) ? 100000000 : 0) + rightInventory.InventoryFreeSpace);

            if (l >= r)
            {
                if (l == 0)
                {
                    Debug.LogWarning("Not enough Inventoryspace to add a Equipment");
                }
                return leftInventory.AddItem(item, obj);
            }
            else
            {
                return rightInventory.AddItem(item, obj);
            }
        }

        public bool CouldAddItem(InventoryType inventoryType, IItem item)
        {
            return GetInv(inventoryType).CouldAddItem(item);
        }

        public bool CouldAddItem(IItem item)
        {
            return leftInventory.CouldAddItem(item) || rightInventory.CouldAddItem(item) || mainInventory.CouldAddItem(item);
        }

        public bool AddItemToEmptyTile(InventoryType inventoryType, IItem item, GameObject obj)
        {
            return GetInv(inventoryType).AddItemToEmptyTile(item, obj);
        }

        public bool AddItemToEmptyTile(IItem item, GameObject obj)
        {
            int l = leftInventory.InventoryFreeSpace;
            int r = rightInventory.InventoryFreeSpace;

            if (leftInventory.InventoryFreeSpace >= rightInventory.InventoryFreeSpace)
            {
                if (l == 0)
                {
                    return mainInventory.AddItemToEmptyTile(item, obj);
                }
                else
                {
                    return leftInventory.AddItemToEmptyTile(item, obj);
                }
            }
            else
            {
                if (r == 0)
                {
                    return mainInventory.AddItemToEmptyTile(item, obj);
                }
                else
                {
                    return rightInventory.AddItemToEmptyTile(item, obj);
                }
            }
        }

        public bool ContainsItem(InventoryType inventoryType, int id)
        {
            return GetInv(inventoryType).ContainsItem(id);
        }

        public bool ContainsItem(int id)
        {
            return leftInventory.ContainsItem(id) || rightInventory.ContainsItem(id) || mainInventory.ContainsItem(id);
        }

        public int FindItem(InventoryType inventoryType, int id)
        {
            return GetInv(inventoryType).FindItem(id);
        }

        public GameObject DropFromInventory(InventoryType inventoryType, int index)
        {
            return GetInv(inventoryType).DropFromInventory(index);
        }

        public GameObject DropFromInventorySilent(InventoryType inventoryType, int index)
        {
            return GetInv(inventoryType).DropFromInventorySilent(index);
        }

        public void AddGameObjectCopyOfItem(InventoryType inventoryType, IItem item, GameObject itemInstance)
        {
            GetInv(inventoryType).AddGameObjectCopyOfItem(item, itemInstance);
        }

        public Inventory GetInv(InventoryType inventoryType)
        {
            switch (inventoryType)
            {
                case InventoryType.Left:
                    return leftInventory;
                case InventoryType.Right:
                    return rightInventory;
                default:
                    return mainInventory;
            }
        }
    }
}
