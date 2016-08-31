using UnityEngine;
using System.Collections;
using Actors;


namespace ItemHandler
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerPickup : MonoBehaviour {

        [SerializeField]
        StaticItem item;

        PlayerActor playerActor;

        void Awake()
        {
            item.ItemDropHandler += Item_ItemDropHandler;
        }

        private void Item_ItemDropHandler(IInventory inv)
        {
            this.enabled = true;
        }

        void Start() {
            playerActor = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
        }

        void OnTriggerEnter2D (Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                if (playerActor.Inventory.AddItem(item, gameObject))
                {
                    this.enabled = false;
                }
            }
        }
    }
}
