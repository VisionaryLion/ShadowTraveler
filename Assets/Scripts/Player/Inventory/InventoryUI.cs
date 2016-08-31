using UnityEngine;
using UnityEngine.UI;

namespace ItemHandler
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        IInventory inventory;
        [SerializeField]
        DisplayItem[] displayItem;
        [SerializeField]
        Text toolTipDisplayer;

        public Text ToolTipDisplayer { get { return toolTipDisplayer; } }

        void Awake()
        {
            for (int iSlot = 0; iSlot < displayItem.Length; iSlot++)
            {
                displayItem[iSlot].inventoryUI = this;
            }
        }

        void Start()
        {
            inventory.InventoryChangeHandler += new Inventory.OnInventoryChanged( UpdateUI);
            for (int iSlot = 0; iSlot < displayItem.Length; iSlot++)
            {
                displayItem[iSlot].slot.SlotIndex = iSlot;
                displayItem[iSlot].UpdateUI();
            }
        }

        public void UpdateUI(IInventory inv)
        {
            for (int iSlot = 0; iSlot < displayItem.Length; iSlot++)
            {
                displayItem[iSlot].UpdateUI();
            }
        }
    }
}
