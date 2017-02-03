using UnityEngine;
using System.Collections;
using ItemHandler;
using Entities;

public interface IInventoryEntity {

    bool TryPickItemUp(ItemEntity itemActor);
    IInventory Inventory { get; }
}
