using UnityEngine;
using System.Collections;
using Actors;
using ItemHandler;

public class EquipItem : MonoBehaviour {

    [SerializeField]
    GameObject itemToEquip;

    [AssignActorAutomaticly]
    BasicEntityEquipmentActor actor;

	void Start ()
    {
        GameObject clone = Instantiate(itemToEquip);
        actor.TwoHandInventory.AddItem(clone.GetComponent<TwoHandItemActor>());        
        actor.TwoHandEquipmentManager.EquipNextItem(false);        
    }
}