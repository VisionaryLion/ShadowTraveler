using UnityEngine;
using System.Collections;

public class AddItem : MonoBehaviour {

    public Inventory.IInventory inv;
    public Inventory.StaticItem item;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Inventory.IItem clonedItem = (Inventory.IItem)item.Clone();
            clonedItem.StackTop = 1;
            inv.AddItem(clonedItem, null);
        }
	}
}
