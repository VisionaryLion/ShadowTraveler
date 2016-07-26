//Add to UI slot prefab
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class SlotHandler : MonoBehaviour, IDropHandler
    {
        public int SlotIndex { set { slotIndex = value; } get { return slotIndex; } }
        private int slotIndex;


        public void OnDrop(PointerEventData eventData)
        {
            DisplayItem droppedItem = eventData.pointerDrag.GetComponent<DisplayItem>();
            droppedItem.inventory.TryMoveItem(droppedItem.slot.slotIndex, slotIndex);
        }
    }
}
