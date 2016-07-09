/*Add to same empty that holds the Inventory script. Make sure to instantiate the InventoryDisplay prefab. 
  Else, ensure you have an inventory panel object that holds the slot panel, which in turn holds the slots. 
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        DisplayItem[] displayItem;

        void Start ()
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
