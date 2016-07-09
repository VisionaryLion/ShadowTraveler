using UnityEngine;
using System.Collections;
namespace Inventory
{
    public abstract class IInventory : MonoBehaviour {

        public abstract IItem GetItem(int index);
        public abstract bool TryMoveItem(int from, int to);
        public abstract bool AddItem(IItem item, GameObject obj);
        public abstract bool AddItemToEmptyTile(IItem item, GameObject obj);
    }
}
