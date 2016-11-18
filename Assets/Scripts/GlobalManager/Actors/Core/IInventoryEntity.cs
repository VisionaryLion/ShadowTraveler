using UnityEngine;
using System.Collections;
using ItemHandler;
using Actors;

public interface IInventoryEntity {

    bool TryPickItemUp(ItemActor itemActor);
    IInventory Inventory { get; }
}
