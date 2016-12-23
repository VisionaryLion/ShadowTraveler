using UnityEngine;
using System.Collections;

public class ObjectFallOnTerrain : MonoBehaviour {
	
	public Transform objectParent;
	
	// Update is called once per frame
	void Update () {
		if (!Input.GetKeyDown(KeyCode.Return))
			return;
			
		int childCount = objectParent.GetChildCount();
		if (objectParent != null && childCount > 0) {
			
			// We need to get all children separatly to tell the API that it must make every one of them fall, no the whole as if it were only one object
			Transform[] childs = new Transform[childCount];
			for (int i=0;i<childCount;i++) {
				childs[i] = objectParent.GetChild(i);
			}
			
			// Make them fall : list of object, true to set the normal from the hit, Collider to tell it to use the collider for raycast
			AlignManager.FallOnTerrain(childs, true, ExtentsGetter.BoundType.Collider);
		}
	}
}
