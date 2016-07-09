//Add to UI item tile prefab
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class DisplayItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        
        [SerializeField]
        Image image;
        [SerializeField]
        Text stackCount;

        public SlotHandler slot;
        public IInventory inventory;
        public IItem HeldItem { get { return inventory.GetItem(slot.SlotIndex); } }

        Vector2 dragOffset;             //Only to ensure that the drag doesnt cause the item icon to snap to center
        Vector2 orgPos;

        public void UpdateUI()
        {
            IItem cachedItem = HeldItem;
            if (cachedItem == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                stackCount.text = cachedItem.GetStackTopString();
                image.sprite = cachedItem.Icon;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            orgPos = transform.position;
            dragOffset = eventData.position - (Vector2)transform.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = orgPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position - dragOffset;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //tooltip.Deactivate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //tooltip.Activate(itemInterface);
        }
    }
}
