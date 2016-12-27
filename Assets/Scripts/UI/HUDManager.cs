using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Entity;


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

        [Header("Right HUD")]
        [SerializeField]
        Text rightItemName;
        [SerializeField]
        Text rightItemDescription;
        [SerializeField]
        Image rightItemIcon;

        PlayerEntity player;
        Sprite emptySprite = null;
        public static HUDManager hudManager;

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
        }

        public void EquipRight(IItem item)
        {
            rightItemDescription.text = item.Description;
            rightItemName.text = item.Title;
            leftItemIcon.enabled = true;
            rightItemIcon.sprite = item.Icon;
        }

        public void EmptyLeft()
        {
            leftItemDescription.text = "";
            leftItemName.text = "";
            leftItemIcon.enabled = false;
        }

        public void EmptyRight()
        {
            rightItemDescription.text = "";
            rightItemName.text = "";
            rightItemIcon.enabled = false;
        }
    }
}





