// by Equilibre Games http://www.equilibregames.com
// Lead: Frederic Rolland-Porche
// listed in unity asset store since version 1.2
//
// Post your feature request on Unity3D forum / Assets and Asset Store / Align Editor !
//
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Function to duplicate a prefab (keeping link) or a gameobject in a 3D grid
/// </summary>
public class DuplicateManager {

	/// <summary>
	/// Duplicate a Prefab or a GameObject in a grid
	/// </summary>
	/// <param name="prefabObject">
	/// The <see cref="GameObject"/> prefab if there is one
	/// </param>
	/// <param name="referenceObject">
	/// The <see cref="GameObject"/> to duplicate
	/// </param>
	/// <param name="transformList">
	/// All <see cref="Transform[]"/> to be distributed
	/// </param>
	/// <param name="sortBy">
	/// The axis which is used to sort the transform list, <see cref="SortAxis"/>
	/// </param>
	/// <param name="axis">
	/// The axis to distribute the transforms on, using the same <see cref="Vector3"/> format
	/// </param>
	public static void DuplicateInGrid(GameObject prefabObject, GameObject referenceObject, Vector3 gridSize, Vector3 offsets, ExtentsGetter.BoundType type)
	{

		// Get the min and max marks
		Vector3 minMarkPosition = referenceObject.transform.position;

		// Total grid object instantiation count
		int gridCount = 0;
		// The steps are here to be sure that we start after the selected object, on the right axis
		int cellX = 0;
		int cellY = 0;
		int cellZ = 0;

		// Calculate the count of elements to create
		gridCount = Mathf.RoundToInt(Mathf.Max(gridSize.x, 1) * Mathf.Max(gridSize.y, 1) * Mathf.Max(gridSize.z, 1));

		// Interval between parts (do not use prefab, as its extents are not calculated by Unity before instantiation)
		Vector3 distanceBetween = ExtentsGetter.GetHierarchyBounds(referenceObject, type).size;

		Vector3 newPos = minMarkPosition;
		// First step, to avoid creating in the same position of the original object
		if (gridSize.x > 1)
			cellX = 1;
		else if (gridSize.y > 1)
			cellY = 1;
		else
			cellZ = 1;

		// ...
		GameObject duplicatedObj = null;
		string baseName = "";
		
		// Starts @ 1 to avoid duplicate the first object (@ the current location)
		for (int i = 1; i < gridCount; i++) {
			// Keep the link to the prefab, if there is one
			if (prefabObject) {
				duplicatedObj = PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;
				baseName = prefabObject.name;
			} else {
				duplicatedObj = (GameObject) GameObject.Instantiate(referenceObject);
				baseName = referenceObject.name;
			}
			// Name the new object from its position in the grid
			duplicatedObj.name = baseName;
			if (gridSize.x > 0)
				duplicatedObj.name += "_" + cellX;
			if (gridSize.y > 0)
				duplicatedObj.name += "_" + cellY;
			if (gridSize.z > 0)
				duplicatedObj.name += "_" + cellZ;

			// Compute object position
			newPos.x = minMarkPosition.x + (distanceBetween.x + offsets.x) * cellX;
			newPos.y = minMarkPosition.y + (distanceBetween.y + offsets.y) * cellY;
			newPos.z = minMarkPosition.z + (distanceBetween.z + offsets.z) * cellZ;
			duplicatedObj.transform.position = newPos;

			// Update the grid indexes
			cellX++;
			if (cellX >= gridSize.x || gridSize.x == 0) {
				cellX = 0;
				cellY++;
				if (cellY >= gridSize.y || gridSize.y == 0) {
					cellY = 0;
					cellZ++;
				}
			}
		}
	}
}
