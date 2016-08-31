using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ItemHandler
{
    public abstract class IInventory : MonoBehaviour {

        public abstract IItem GetItem(int index);
        public abstract bool TryMoveItem(int from, int to);
        public abstract bool AddItem(IItem item, GameObject obj);
        public abstract bool AddItemToEmptyTile(IItem item, GameObject obj);
        public abstract bool ContainsItem(int id);
        public abstract int FindItem(int id);
        public abstract GameObject DropFromInventory(int index);
        public abstract GameObject EquipItem(int index);
        public abstract void AddGameObjectCopyOfItem(IItem item, GameObject itemInstance);

        public delegate void OnInventoryChanged(IInventory inventory);
        public event OnInventoryChanged InventoryChangeHandler;

        public void InvokeOnInventoryChanged(IInventory inv)
        {
            if(InventoryChangeHandler != null)
            InventoryChangeHandler.Invoke(inv);
        }
    }
}
