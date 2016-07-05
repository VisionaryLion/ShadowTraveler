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