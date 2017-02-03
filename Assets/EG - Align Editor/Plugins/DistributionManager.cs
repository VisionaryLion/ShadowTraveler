// by Equilibre Games http://www.equilibregames.com
// Lead: Frederic Rolland-Porche
// listed in unity asset store since version 1.2
//
// Post your feature request on Unity3D forum / Assets and Asset Store / Align Editor !
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Functions to distribute Position, Rotation and Local Scale from a Transform Array
/// </summary>
public class DistributionManager {

	public enum SortAxis {None, X, Y, Z};

	/// <summary>
	/// Simplified linear position distribution : the first object is used as reference, no sorting object, align on the X axis, do not use boundaries
	/// </summary>
	/// <param name="transformList">
	/// All <see cref="Transform[]"/> to be distributed from the first one in the list
	/// </param>
	public static void DistributePosition(Transform[] transformList) {
		if (transformList.Length > 0)
			DistributePosition(transformList[0], transformList, SortAxis.None, Vector3.right);
	}

	/// <summary>
	/// Linear Position Distribution with full parameters
	/// </summary>
	/// <param name="activeTransform">
	/// The <see cref="Transform"/> that other transforms will be aligned to, if requested
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
	public static void DistributePosition(Transform activeTransform, Transform[] transformList, SortAxis sortBy, Vector3 axis) {
		if (transformList.Length == 0)
			return;

		// Get the min and max marks (no bounds here since it would make the ends go farer)
		Vector3 minMarkPosition = ExtentsGetter.GetMinMarkPosition(activeTransform.position, transformList, ExtentsGetter.BoundType.None);
		Vector3 maxMarkPosition = ExtentsGetter.GetMaxMarkPosition(activeTransform.position, transformList, ExtentsGetter.BoundType.None);

		// Interval
		Vector3 distanceBetween = (maxMarkPosition - minMarkPosition) / (transformList.Length - 1);
		// Delta from minMark
		Vector3 delta = Vector3.zero;

		// List of selected objects, to sort from the min position to the max position
		List<Transform> list = new List<Transform>(transformList);

		// Sort the selected objects from their position (only one axis is taken into account)
		switch (sortBy) {
		case SortAxis.Z:
			list.Sort(ByVector3PositionZ);
			break;
		case SortAxis.Y:
			list.Sort(ByVector3PositionY);
			break;
		default:
			list.Sort(ByVector3PositionX);
			break;
		}

		foreach (Transform nextTransform in list) {
			Vector3 newPos;
			newPos.x = (axis == Vector3.one || axis == Vector3.right) ? minMarkPosition.x + delta.x : nextTransform.position.x;
			newPos.y = (axis == Vector3.one || axis == Vector3.up) ? minMarkPosition.y + delta.y : nextTransform.position.y;
			newPos.z = (axis == Vector3.one || axis == Vector3.forward) ? minMarkPosition.z + delta.z : nextTransform.position.z;
			nextTransform.position = newPos;
			delta += distanceBetween;
		}
	}

	/// <summary>
	/// Linear Rotation distribution
	/// </summary>
	/// <param name="activeTransform">
	/// The <see cref="Transform"/> to use as the first transform
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
	public static void DistributeRotation(Transform activeTransform, Transform[] transformList, SortAxis sortBy, Vector3 axis) {
		if (transformList.Length == 0)
			return;

		// Get the min and max marks
		Vector3 minMarkRotation = BasicExtents.GetMinMarkRotation(activeTransform.eulerAngles, transformList);
		Vector3 maxMarkRotation = BasicExtents.GetMaxMarkRotation(activeTransform.eulerAngles, transformList);
		
		// Interval
		Vector3 distanceBetween = (maxMarkRotation - minMarkRotation) / (transformList.Length - 1);
		// Delta from minMark
		Vector3 delta = Vector3.zero;
		
		// List of selected objects, to sort from the min position to the max position
		List<Transform> list = new List<Transform>(transformList);
		
		// Sort the selected objects as requested
		switch (sortBy) {
		case SortAxis.Z:
			list.Sort(ByVector3PositionZ);
			break;
		case SortAxis.Y:
			list.Sort(ByVector3PositionY);
			break;
		default:
			list.Sort(ByVector3PositionX);
			break;
		}

		foreach (Transform nextTransform in list) {
			Vector3 newPos;
			newPos.x = (axis == Vector3.one || axis == Vector3.right) ? minMarkRotation.x + delta.x : nextTransform.eulerAngles.x;
			newPos.y = (axis == Vector3.one || axis == Vector3.up) ? minMarkRotation.y + delta.y : nextTransform.eulerAngles.y;
			newPos.z = (axis == Vector3.one || axis == Vector3.forward) ? minMarkRotation.z + delta.z : nextTransform.eulerAngles.z;
			nextTransform.rotation = Quaternion.Euler(newPos);
			delta += distanceBetween;
		}
	}

	/// <summary>
	/// Linear (local) Scale distribution
	/// </summary>
	/// <param name="activeTransform">
	/// The <see cref="Transform"/> to use as the first transform
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
	public static void DistributeScale(Transform activeTransform, Transform[] transformList, SortAxis sortBy, Vector3 axis) {
		if (transformList.Length == 0)
			return;

		// Get the min and max marks
		Vector3 minMarkScale = BasicExtents.GetMinMarkScale(activeTransform.localScale, transformList);
		Vector3 maxMarkScale = BasicExtents.GetMaxMarkScale(activeTransform.localScale, transformList);

		// Interval
		Vector3 distanceBetween = (maxMarkScale - minMarkScale) / (transformList.Length - 1);
		// Delta from minMark
		Vector3 delta = Vector3.zero;

		// List of selected objects, to sort from the min position to the max position
		List<Transform> list = new List<Transform>(transformList);

		// Sort the selected objects as requested
		switch (sortBy) {
		case SortAxis.Z:
			list.Sort(ByVector3PositionZ);
			break;
		case SortAxis.Y:
			list.Sort(ByVector3PositionY);
			break;
		default:
			list.Sort(ByVector3PositionX);
			break;
		}

		foreach (Transform nextTransform in list) {
			Vector3 newPos;
			newPos.x = (axis == Vector3.one || axis == Vector3.right) ? minMarkScale.x + delta.x : nextTransform.localScale.x;
			newPos.y = (axis == Vector3.one || axis == Vector3.up) ? minMarkScale.y + delta.y : nextTransform.localScale.y;
			newPos.z = (axis == Vector3.one || axis == Vector3.forward) ? minMarkScale.z + delta.z : nextTransform.localScale.z;
			nextTransform.localScale = newPos;
			delta += distanceBetween;
		}
	}

	/// <summary>
	/// Grid position distribution with full parameters
	/// </summary>
	/// <param name="activeTransform">
	/// The <see cref="Transform"/> that other transforms will be aligned to, if requested
	/// </param>
	/// <param name="transformList">
	/// All <see cref="Transform[]"/> to be distributed
	/// </param>
	/// <param name="gridSize">
	/// The grid size in a <see cref="Vector3"/> so the objects can be distributed in cells
	/// </param>
	public static void DistributePositionInGrid(Transform activeTransform, Transform[] transformList, Vector3 gridSize) {
		if (transformList.Length == 0)
			return;

		// Get the min and max marks
		Vector3 minMarkPosition = ExtentsGetter.GetMinMarkPosition(activeTransform.position, transformList, ExtentsGetter.BoundType.None);
		Vector3 maxMarkPosition = ExtentsGetter.GetMaxMarkPosition(activeTransform.position, transformList, ExtentsGetter.BoundType.None);

		// List of selected objects, to sort from the min position to the max position
		List<Transform> list = new List<Transform>(transformList);

		int gridCount = list.Count;
		// the distance between each cell is calculated from the min and max positionned objects
		Vector3 distanceBetween = Vector3.one;
		distanceBetween.x = (gridSize.x > 1 ? ((maxMarkPosition.x - minMarkPosition.x) / (gridSize.x - 1)) : 0);
		distanceBetween.y = (gridSize.y > 1 ? ((maxMarkPosition.y - minMarkPosition.y) / (gridSize.y - 1)) : 0);
		distanceBetween.z = (gridSize.z > 1 ? ((maxMarkPosition.z - minMarkPosition.z) / (gridSize.z - 1)) : 0);

		Vector3 newPos = Vector3.zero;
		Transform nextTransform = null;

		for (int i = 0; i < gridCount; i++) {
			nextTransform = list[i];

			newPos = nextTransform.position;
			if (gridSize.x > 0)
				newPos.x = Mathf.RoundToInt((nextTransform.position.x - minMarkPosition.x) / distanceBetween.x) * distanceBetween.x + minMarkPosition.x;
			if (gridSize.y > 0)
				newPos.y = Mathf.RoundToInt((nextTransform.position.y - minMarkPosition.y) / distanceBetween.y) * distanceBetween.y + minMarkPosition.y;
			if (gridSize.z > 0)
				newPos.z = Mathf.RoundToInt((nextTransform.position.z - minMarkPosition.z) / distanceBetween.z) * distanceBetween.z + minMarkPosition.z;

			nextTransform.position = newPos;
		}
	}

	// Comparer method : X axis
	public static int ByVector3PositionX(Transform leftMost, Transform rightMost) {
		return Mathf.RoundToInt(leftMost.position.x - rightMost.position.x);
	}
	// Comparer method : Y axis
	public static int ByVector3PositionY(Transform leftMost, Transform rightMost) {
		return Mathf.RoundToInt(leftMost.position.y - rightMost.position.y);
	}
	// Comparer method : Z axis
	public static int ByVector3PositionZ(Transform leftMost, Transform rightMost) {
		return Mathf.RoundToInt(leftMost.position.z - rightMost.position.z);
	}

}
