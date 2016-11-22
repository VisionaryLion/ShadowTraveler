using UnityEngine;
using System.Collections;
using ItemHandler;
using System;

namespace Actors
{
    public class BasicEntityEquipmentActor : BasicEntityActor, IInventoryEntity
    {
        [SerializeField]
        TwoHandInventory twoHandInventory;
        [SerializeField]
        TwoHandEquipmentManager twoHandEquipmentManager;

        public TwoHandInventory TwoHandInventory { get { return twoHandInventory; } }
        public TwoHandEquipmentManager TwoHandEquipmentManager { get { return twoHandEquipmentManager; } }
        public IInventory Inventory { get { return twoHandInventory; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            twoHandInventory = GetComponentInChildren<TwoHandInventory>();
            twoHandEquipmentManager = GetComponentInChildren<TwoHandEquipmentManager>();
        }
#endif

        public bool TryPickItemUp(ItemActor itemActor)
        {
            return twoHandEquipmentManager.TryPickMeUp(itemActor);
        }
    }
}
