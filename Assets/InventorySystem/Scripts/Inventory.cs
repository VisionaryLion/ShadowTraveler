/*
 * Inventory script. Add to an empty and parent it to the player.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int inventorySize; 			//Number of distinct elements the inventory can support 
    List<GameObject> inventoryCache;

    //Add reference to Inventory Display component 
    InventoryDisplay invDisplay;

    void Awake()
    {
        //Assign Inventory display component to the reference
        inventoryCache = new List<GameObject>();
        invDisplay = GetComponent<InventoryDisplay>();
    }

    //Checks and adds item tothe cache if possible
    public bool AddItem(GameObject item) //TODO: Add system of return codes
    {
        bool toStack = false;
        //int itemIndex = -1;
        GameObject itemStacked = null;
        ItemInterface itemIE = item.GetComponent<ItemInterface>();
        if(!itemIE)
        {
            Debug.LogError("Couldnt find item interface component on object");
            return false;
        }
        if (itemIE.isStackable)
        {
            foreach (GameObject go in inventoryCache)
            {
                if (go.GetComponent<ItemInterface>().itemID == item.GetComponent<ItemInterface>().itemID
                    && !go.GetComponent<ItemInterface>().StackLimitReached())
                {
                    itemStacked = go;
                    toStack = true;
                }
            }
        }
        if (toStack)
        {
            Debug.Log("Stacking.");
            if (itemStacked)
            {
                itemStacked.GetComponent<ItemInterface>().stackTop++;
                itemStacked.GetComponent<ItemInterface>().displayItem.Number = itemStacked.GetComponent<ItemInterface>().stackTop.ToString();

                Debug.Log("itemStacked:"+itemStacked.GetComponent<ItemInterface>().itemEquipSlot);
                Debug.Log("item:"+item.GetComponent<ItemInterface>().itemEquipSlot);

                Destroy(item); 		//verify. Find another way to handle this
                return true;
            }
            else
            {
                Debug.LogError("Couldn't find element to stack.");
                return false;
            }
        }
        else
        {
            if (inventoryCache.Count >= inventorySize)
            {  //Stack of validations. Only checks for limit as of now.
                Debug.Log("Inventory limit reached.");
                return false;
            }
            GameObject dispSlot = invDisplay.FindEmptySlot();//Grab empty slot in display
            
            if (!dispSlot)// Validation
            {
                Debug.LogError("Couldnt find an empty slot to attach to.");
                return false;
            }
            invDisplay.AddItemToSlot(dispSlot,item.GetComponent<ItemInterface>()); //Create Display item object based on ItemInterface
            DisplayItem dispItem = dispSlot.transform.GetComponentInChildren<DisplayItem>();
            if(!dispItem)
            {
                Debug.LogError("Couldnt find item with DisplayItem component attached in slot.");
                return false;
            }
            //Assign item interface and Displayitem references to each other
            item.GetComponent<ItemInterface>().displayItem = dispItem;
            dispItem.itemInterface = item.GetComponent<ItemInterface>();
            //Sync parameters 
            dispItem.Sync();
            
            inventoryCache.Add(item);
            item.GetComponent<ItemInterface>().DisableAndMove();//Storing reference to object picked up
            //TODO:Update inventory display
            return true;
        }
    }

    //Find and delete from cache
    bool RemoveItemFromCache(GameObject item)
    {
        int indexRm=inventoryCache.IndexOf(item);
        if(indexRm < 0)
        {
            Debug.Log("Couldnt find element to remove.");
            return false;
        }
        inventoryCache.RemoveAt(indexRm);
        //TODO: Update inv display
        return true;
    }

    //Drop an item from inventory
    bool DropFromInventory(GameObject item, ref string status)
    {
        GameObject toDrop=null;  //Incase of a stackable item, 
        //need to create an instance of the saved object and then drop it.  
        ItemInterface ie = item.GetComponent<ItemInterface>();
        if (!ie) 
        {
            status = "ItemInterface not found on object.";
            return false;
        }
        if(ie.stackTop==1)
        {
            toDrop = item;
            if(!RemoveItemFromCache(item))
            {
                Debug.Log("Couldn't clear cache");
            }
        }
        else
        {
            ie.stackTop -= 1;
            ie.displayItem.Number = ie.stackTop.ToString();
            toDrop = (GameObject)Instantiate(item,item.transform.position,item.transform.rotation);
        }

        toDrop.SetActive(true);
        if(toDrop.GetComponent<MeshRenderer>()!=null)
        {
            toDrop.GetComponent<MeshRenderer>().enabled = true;
        }
        if(toDrop.GetComponent<Collider2D>()!=null)
        {
            toDrop.GetComponent<Collider2D>().enabled = true;
        }
        toDrop.GetComponent<ItemInterface>().enabled = true;
        toDrop.GetComponent<Transform>().parent = null; //TODO: Possible source of bugs. Assign to parent of player, ideally.
        return true;
    }
}