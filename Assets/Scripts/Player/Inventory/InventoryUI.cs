using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryUI : MonoBehaviour
    {
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
            for (int iSlot = 0; iSlot < displayItem.Length; iSlot++)
            {
                displayItem[iSlot].slot.SlotIndex = iSlot;
                displayItem[iSlot].UpdateUI();
            }
        }

        public void UpdateUI()
        {
            for (int iSlot = 0; iSlot < displayItem.Length; iSlot++)
            {
                displayItem[iSlot].UpdateUI();
            }
        }
    }
}
