using UnityEngine;
using System.Collections;
using ItemHandler;
using Entity;

public interface IInventoryEntity {

    bool TryPickItemUp(ItemEntity itemActor);
    IInventory Inventory { get; }
}
