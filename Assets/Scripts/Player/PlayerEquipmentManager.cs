using UnityEngine;
using Actors;
using System.Collections;
using System;

namespace ItemHandler
{
    public class PlayerEquipmentManager : MonoBehaviour
    {

        [SerializeField]
        EquipmentButtonBinding[] binds; //according to keyboard numbers
        [SerializeField]
        public Transform equipSpawnPoint;

        public bool allowInput;

        [AssignActorAutomaticly]
        [SerializeField]
        [HideInInspector]
        PlayerActor actor;

        GameObject currentPhysicalEquipment;
        IItem currentEquipedItem;

        // Update is called once per frame
        void Update()
        {
            if (!allowInput)
                return;
            foreach (EquipmentButtonBinding b in binds)
            {
                if (Input.GetKeyDown(b.key) && actor.Inventory.ContainsItem(b.equipment.itemID))
                {
                    if (currentEquipedItem != null)
                    {
                        actor.Inventory.AddGameObjectCopyOfItem(currentEquipedItem, currentPhysicalEquipment);
                    }

                    int itemIndex = actor.Inventory.FindItem(b.equipment.itemID);
                    currentEquipedItem = actor.Inventory.GetItem(itemIndex);
                    currentPhysicalEquipment = actor.Inventory.EquipItem(itemIndex);
                    currentPhysicalEquipment.transform.position = equipSpawnPoint.position;
                    currentPhysicalEquipment.transform.rotation = equipSpawnPoint.rotation;
                    currentPhysicalEquipment.transform.parent = equipSpawnPoint;
                }
            }
        }

        [Serializable]
        public class EquipmentButtonBinding
        {
            public KeyCode key;
            public ItemData equipment;
        }
    }
}
