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

        [Header("Right HUD")]
        [SerializeField]
        Text rightItemName;
        [SerializeField]
        Text rightItemDescription;
        [SerializeField]
        Image rightItemIcon;

        PlayerActor player;
        Sprite emptySprite = null;

        void Awake()
        {
            player = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
        }

        void Update()
        {
            if (player.GetComponentInChildren<ItemHolder>() != null)
            {
                if (!leftItemIcon.enabled)
                {
                    leftItemIcon.enabled = true;
                }                

                leftItemName.text = player.GetComponentInChildren<ItemHolder>().item.Title;
                leftItemIcon.sprite = player.GetComponentInChildren<ItemHolder>().item.Icon;
                leftItemDescription.text = player.GetComponentInChildren<ItemHolder>().item.Description;

            }
            else
            {
                leftItemName.text = "";
                leftItemDescription.text = "";
                leftItemIcon.enabled = false;
            }                      
        }

        /* Once new system implemented

        void EquipLeft(StaticItem item)  // or player.EquipmentManager.CurrentEquipedGameObjectLeft
        {
            leftItemDescription.text = "";
            leftItemName.text = "";
            leftItemIcon.enabled = true;
            leftItemIcon.sprite = null;
        }
        
        void EmptyLeft(StaticItem item)
        {
            leftItemDescription.text = "";
            leftItemName.text = "";
            leftItemIcon.enabled = false;
        }

        void EquipRight(StaticItem item)
        {
            rightItemDescription.text = item.Description;
            rightItemName.text = item.Title;
            leftItemIcon.enabled = true;
            rightItemIcon.sprite = item.Icon;
        }

        void EmptyRight(StaticItem item)
        {
            rightItemDescription.text = "";
            rightItemName.text = "";
            rightItemIcon.enabled = false;
        }
        
        */


    }
}





