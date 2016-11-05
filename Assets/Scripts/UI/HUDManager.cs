using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Actors;


namespace ItemHandler
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField]
        float updateRate = 10f;

        [SerializeField]
        Text itemName;
        [SerializeField]
        Text itemDescription;
        [SerializeField]
        Image itemIcon;

        PlayerActor player;
        Sprite emptySprite = null;



        void Awake()
        {
            player = ActorDatabase.GetInstance().FindFirst<PlayerActor>();                        
        }

        void Start()
        {
            updateInfo();
            StartCoroutine(startUpdate());
        }
        
        void updateInfo()
        {
            if (player.GetComponentInChildren<ItemHolder>() != null)
            {
                if (!itemIcon.enabled)
                {
                    itemIcon.enabled = true;
                }
                                
                itemName.text = player.GetComponentInChildren<ItemHolder>().item.Title;
                itemIcon.sprite = player.GetComponentInChildren<ItemHolder>().item.Icon;
                itemDescription.text = player.GetComponentInChildren<ItemHolder>().item.Description;

            } else
            {
                itemName.text = "";
                itemDescription.text = "";
                itemIcon.enabled = false;
            }
            
        }
        IEnumerator startUpdate()
        {
            yield return new WaitForSeconds(1 / updateRate);
            updateInfo();
            StartCoroutine(startUpdate());
        }

    }
}