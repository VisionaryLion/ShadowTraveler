TODO:
- handle deletion interface.
- handle dropping interface.
- implement tooltip
- implement inventory menu on right click that will give you options to delete/use/equip/drop.
- Handle dragging/deletion of stackable objects. Whoile tileset is moved as of now.
- Item management:
	- Have to implement character screen for equipped items.
	- need to add a way to generate item parameters from JSON code/ XML data.
	- need to implement code for ItemEffect for consumables and ItemEquip for equipment.
- clean up logs


Sanity testing:
- Add objects. Two stackable and two unstackable. Stackable elements need multiple copies.
- Add all of them to inventory. 
- Ensure that each tile can be moved to each of the other slots. Ensure parent reference and slot reference to items are updated. Ensure inventory cache is maintained on each operation. 
- Switch each of the tiles with the other and repeat observations
- 


Cover testing:

//Yet to implement

//Oribows Edits
Didn't comment things. A working setup is in the InventoryTest1.scene scene. To add an item to inventory press 'A'. This debug function
is handled by the AddItem.cs script. The backend now works in that way: 

ItemData
- stores non changing data

SomeClass (like StaticItem)
- holds a ref to a static data holder (like ItemData)
- stores some instances related vars
- implements IItem

IItem
- interface for the inventory to get item related data

ItemHolder
- simple Mono script, that holds a ref to an IItem
- to bind Item data to a GameObject, but make IItem independent and movable (for pooling reasons)

IInventory
- manly dictates methods of data changing and accessing
- not all functions implemented

Inventory
- implements IInventory
- deals with pooling of item GameObjects once added
- holds a ref to InventoryUI and calls UpdateUI when a change happens (except when an Item is moved, as this is handled by the UI itself)

InventoryUI
- main UI controller
- updates representation of DisplayItems

DisplayItem
- displays a certain item
- handles dragging, but never actually changes original position, just the ItemID gets swapped
- ref to a SlotHandler

SlotHandler
- gets OnDrop events
- holds a inventory index

TODO:
- on dragging, the dragged item falsely receives the OnDrop event, because it's the deepest in the hierarchy
- on dragging, the dragged item sometimes get occluded by other UI stuff
- let the description text be displayed, by adding text GameObject to DisplayItem and let DisplayItem updated that (same for other things)
- add tooltips -> just premade on ToolTipp, then move it to where the mouse is and let the InventoryUI hold a ref to that
  this is efficient, because there can only be one ToolTipp at a time and it has to move where the mouse is anyway

- some day later, add a InventoryActor

																		