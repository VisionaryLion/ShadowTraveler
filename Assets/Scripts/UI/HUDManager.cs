using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Actors;


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

        PlayerActor player;
        Sprite emptySprite = null;
        public static HUDManager hudManager;

        public bool decreaseLeft = false;
        public bool decreaseRight = false;

        DurableItem leftDurableItem;
        DurableItem rightDurableItem;

        void Awake()
        {
            hudManager = this;
            player = ActorDatabase.GetInstance().FindFirst<PlayerActor>();

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
            leftItemDurability.fillAmount = 0;
        }

        public void EmptyRight()
        {
            rightItemDescription.text = "";
            rightItemName.text = "";
            rightItemIcon.enabled = false;
            rightItemDurability.fillAmount = 0;
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
    }
}