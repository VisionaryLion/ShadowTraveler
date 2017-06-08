using UnityEngine;
using System.Collections;
using Entities;
using ItemHandler;

public class EquipItem : MonoBehaviour {

    [SerializeField]
    GameObject itemToEquip;

    [AssignEntityAutomaticly]
    ActingEquipmentEntity actor;

    StaticItemHolder item;

	void Start ()
    {
        GameObject clone = Instantiate(itemToEquip);
        item = clone.GetComponent<StaticItemHolder>();
        actor.TwoHandInventory.AddItem(clone.GetComponent<TwoHandItemEntity>());        
        actor.TwoHandEquipmentManager.EquipNextItem(false);
        Destroy(clone);
    }
}