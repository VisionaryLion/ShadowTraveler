using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DisplayItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler {

    public ItemInterface itemInterface;
    Sprite displayIcon;
    string number;

    public GameObject originalParentSlot;
    public GameObject slotToMoveTo;
    Vector2 dragOffset;             //Only to ensure that the drag doesnt cause the item icon to snap to centre

    public string Number
    {
        get { return number; }
        set 
        { 
            number = value;
            UpdateCount();
        }
    }

    GameObject countText; 

    void Awake()
    {
        countText = gameObject.transform.FindChild("Count").gameObject;
        if (!countText)
            Debug.LogError("Couldn't find countText");
        number = "";        
    }

    public void Sync()
    {
        if (itemInterface)
        {
            displayIcon = itemInterface.displaySprite;
            if (itemInterface.stackTop == 1)
                number = "";
            else
                number = itemInterface.stackTop.ToString();
            UpdateCount();
            SetImage();
        }
        else
            Debug.Log("No item assigned.");
    }

    void UpdateCount()
    {
        if (!countText)
            Debug.LogError("Couldn't find text component");
        if(countText && countText.GetComponent<Text>())
        {
            countText.GetComponent<Text>().text = number;
        }
        else
        {
            Debug.LogError("Couldnt find child with Text component");
        }
    }

    void SetImage()
    {
        if(displayIcon && gameObject.GetComponent<Image>())
        {
            gameObject.GetComponent<Image>().sprite = displayIcon;
        }
        else
        {
            Debug.LogError("Couldnt find Image component");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        originalParentSlot = transform.parent.gameObject;
        slotToMoveTo = originalParentSlot;
        transform.SetParent(originalParentSlot.transform.parent);
        this.transform.position = eventData.position - dragOffset;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        transform.SetParent(slotToMoveTo.transform);
        transform.position = slotToMoveTo.transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        this.transform.position = eventData.position - dragOffset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragOffset = eventData.position - new Vector2(transform.position.x, transform.position.y);
        this.transform.position = eventData.position - dragOffset;
    }
}
