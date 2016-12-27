using UnityEngine;
using System.Collections;
using Entities;
using ItemHandler;

public class EquipItem : MonoBehaviour {

    [SerializeField]
    GameObject itemToEquip;

    [AssignEntityAutomaticly]
    ActingEquipmentEntity actor;

	void Start ()
    {
        GameObject clone = Instantiate(itemToEquip);
        actor.TwoHandInventory.AddItem(clone.GetComponent<TwoHandItemEntity>());        
        actor.TwoHandEquipmentManager.EquipNextItem(false);        
    }
}