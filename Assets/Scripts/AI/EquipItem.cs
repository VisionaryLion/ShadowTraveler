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
        actor.TwoHandEquipmentManager.TryPickMeUp(clone.GetComponent<TwoHandItemActor>());       
    }
}