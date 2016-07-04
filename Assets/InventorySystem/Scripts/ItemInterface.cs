//Attach as component to an object that needs to be added as an inventory item. 
//Initialise members of class from main object once it is ready. Keeping it public for now.

using UnityEngine;
using System.Collections;

public class ItemInterface : MonoBehaviour
{
    //to be populated from object attached to. Keeping it public for now.
    public string itemID;
    public Sprite displaySprite;
    public string itemEquipSlot; //In case of weapons or equipment, can be used to assign to specific slots
    public bool isEquipment; //Weapon or Equipment that can be dragged to character slots.
    public bool isConsumable; //Items that need to be destroyed after use
    public bool isStackable=false; //Items that can be stacked atop each other.
    public int stackLimit=20; //Max number of elements that can be stacked onto a slot.
    public int stackTop=1;
    public bool canBeAcquired=true;// In case we need to disable the element from being acquired temporarily


    public GameObject itemHolder; //Reference to object that the item gets parented to.

    public Inventory inv; //Access to inventory class 

    public DisplayItem displayItem;

    


    void Awake()
    {
        if (inv == null)
        {
            inv = (Inventory)FindObjectOfType(typeof(Inventory)); 		//Works under presumption that the player is the only GameObject having an Inventory component
        }
        if (inv == null)
        {
            Debug.LogError("Inventory could not be found. Object cannot be added.");
            canBeAcquired = false;
        }
        else 
        {
            if (itemHolder == null)
            {
                itemHolder = inv.gameObject;
            }
        }
        //Initialise public members once the Item class is decided
    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //Temporary Method to test acquisition. Acquires object if it is clicked on
    void OnMouseDown()
    {
        PickMeUp();
    }

    public void PickMeUp() 
    {
        Debug.Log("Calling PickMeUp() on " + gameObject.name);
        if(!canBeAcquired)
        {
            Debug.Log("Object cant be acquired.");
            return;
        }
        inv.AddItem(gameObject);
    }

    public void DisableAndMove() 
    {
        if(GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        if(GetComponent<Collider2D>()!=null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        GetComponent<ItemInterface>().enabled = false;
        transform.parent = itemHolder.transform;
        transform.localPosition = Vector3.zero;
    }

    public bool StackLimitReached()
    {
        return stackTop >= stackLimit;
    }
}
