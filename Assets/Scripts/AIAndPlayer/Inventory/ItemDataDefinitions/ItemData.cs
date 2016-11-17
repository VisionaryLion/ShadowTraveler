using UnityEngine;

namespace ItemHandler
{
    public class ItemData : ScriptableObject
    {
        public int itemID;
        public Sprite displaySprite;
        public GameObject itemPrefab;
        public bool isEquipment; //Weapon or Equipment that can be dragged to character slots.
        public bool isConsumable; //Items that need to be destroyed after use
        public bool isStackable = false; //Items that can be stacked atop each other.
        public bool canHoldOnlyOne = false; //Items that can be hold only one at a time.
        public int stackLimit = 20; //Max number of elements that can be stacked onto a slot.
        public int poolLimit = 0; //How many copies of this object should max be saved by an inventory.
        public string itemName;
        [Multiline]
        public string description;
        [Multiline]
        public string tooltips;
    }
}
