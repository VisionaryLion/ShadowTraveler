using UnityEngine;
using System.Collections;
using ItemHandler;

namespace Actors
{
    public class BasicEntityWithEquipmentActor : BasicEntityActor
    {
        [SerializeField]
        PlayerInventory multiSlotsInventory;
        [SerializeField]
        EquipmentManager equipmentManager;

        public PlayerInventory MultiSlotsInventory { get { return multiSlotsInventory; } }
        public EquipmentManager EquipmentManager { get { return equipmentManager; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            multiSlotsInventory = GetComponentInChildren<PlayerInventory>();
            equipmentManager = GetComponentInChildren<EquipmentManager>();
        }
#endif
    }
}
