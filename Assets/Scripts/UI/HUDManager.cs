using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Entities;


namespace ItemHandler
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Left HUD")]
        [SerializeField]
        Text leftItemName;
        [SerializeField]
        Text leftItemDescription;
        [SerializeField]
        Image leftItemIcon;
        [SerializeField]
        Image leftItemDurability;

        [Header("Right HUD")]
        [SerializeField]
        Text rightItemName;
        [SerializeField]
        Text rightItemDescription;
        [SerializeField]
        Image rightItemIcon;
        [SerializeField]
        Image rightItemDurability;


        PlayerEntity player;
        Sprite emptySprite = null;
        public static HUDManager hudManager;

        DurableItem leftDurableItem;
        DurableItem rightDurableItem;

        public bool decreaseLeft = false;
        public bool decreaseRight = false;

        void Awake()
        {
            hudManager = this;
            player = EntityDatabase.GetInstance().FindFirst<PlayerEntity>();

            EmptyLeft();
            EmptyRight();
        }


        public void EquipLeft(IItem item)  // or player.EquipmentManager.CurrentEquipedGameObjectLeft
        {
            leftItemDescription.text = item.Description;
            leftItemName.text = item.Title;
            leftItemIcon.enabled = true;
            leftItemIcon.sprite = item.Icon;
            leftItemDurability.fillAmount = 1f;
        }

        public void EquipRight(IItem item)
        {
            rightItemDescription.text = item.Description;
            rightItemName.text = item.Title;
            leftItemIcon.enabled = true;
            rightItemIcon.sprite = item.Icon;
            rightItemDurability.fillAmount = 1f;
        }

        public void EmptyLeft()
        {
            leftItemDescription.text = "";
            leftItemName.text = "";
            leftItemIcon.enabled = false;
            leftItemDurability.fillAmount = 0f;
        }

        public void EmptyRight()
        {
            rightItemDescription.text = "";
            rightItemName.text = "";
            rightItemIcon.enabled = false;
            rightItemDurability.fillAmount = 0f;
        }

        void Update()
        {
            if (decreaseLeft)
            {
                leftItemDurability.fillAmount = leftDurableItem.duration / 100;
            }

            if (decreaseRight)
            {
                rightItemDurability.fillAmount = rightDurableItem.duration / 100;
            }
        }

        public void startBurn(bool right, DurableItem item)
        {
            if (right)
            {
                decreaseRight = true;
                rightDurableItem = item;
            }
            else
            {
                decreaseLeft = true;
                leftDurableItem = item;
            }
        }
    }
}





