//Add to UI item tile prefab
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DisplayItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public ItemInterface itemInterface;
    Sprite displayIcon;
    string number;

    private Tooltip tooltip;

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
        tooltip = (Tooltip)GameObject.FindObjectOfType(typeof(Tooltip));
        if (!tooltip)
            Debug.LogError("Couldnt find tooltip");
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
        Debug.Log("Started drag on slot:"+originalParentSlot.GetComponent<SlotHandler>().SlotID);
        slotToMoveTo = null;
        transform.SetParent(originalParentSlot.transform.parent);
        this.transform.position = eventData.position - dragOffset;

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!slotToMoveTo)//If cant recognise item that it is dropped onto
        {
            Debug.Log("New slot to move to not set. Reverting to older parent");
            parentToSlot(originalParentSlot);
        }
        //The below segment handles the cases where the item is returned to a certain slot. 
        //For deletion, treat it  as an exception and return from an if

        parentToSlot(slotToMoveTo);         //Default case when dropped on a non empty slot. slotMoveTo is set in the slotHandler code.
        slotToMoveTo = null;
        originalParentSlot = transform.parent.gameObject;
        GetComponent<CanvasGroup>().blocksRaycasts = true; 
    }

    void parentToSlot(GameObject moveTo)
    {
        transform.SetParent(moveTo.transform);
        transform.position = moveTo.transform.position;        
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

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(itemInterface);        
    }
}
