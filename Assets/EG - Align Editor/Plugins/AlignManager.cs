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
/// Functions to align position, rotation or scale and simulate a fall on a terrain from a Transform Array
/// New in 2.0 : rotate game object to terrain angle parameter
/// </summary>
public class AlignManager {

	/// <summary>
	/// Used to set the type of alignment
	/// </summary>
	public enum Landmark {
		minimum,
		mean,
		maximum,
		distributed
	}

	/// <summary>
	/// Align all transforms position with one Transform, or on mini/max values of boundaries
	/// </summary>
	/// <param name="referenceTransform">
	/// A <see cref="Transform"/> which all other transforms will be aligned with, if alignType is "mean"
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	/// <param name="axis">
	/// The axis to align on : <see cref="Vector3.one"/> to align all axis, <see cref="Vector3.right"/> to align on the X axis, <see cref="Vector3.up"/> to align on the Y axis, <see cref="Vector3.forward"/> to align on the Z axis
	/// </param>
	/// <param name="alignType">
	/// Witch type of alignement we do, from the <see cref="Landmark"/> enum
	/// </param>
	public static void AlignPosition(Transform referenceTransform, Transform[] transformList, Vector3 axis, Landmark alignType, ExtentsGetter.BoundType boundType) {
		//bool useBounds = (alignType != Landmark.distributed && alignType != Landmark.mean);
		// Get the position from the active selected object
		Vector3 markPosition = referenceTransform.position;
		// If alignment is not the mean one, search the min or max positioned object
		if (alignType == Landmark.minimum) {
			markPosition = ExtentsGetter.GetMinMarkPosition(referenceTransform.position, transformList, boundType);
		} else if (alignType == Landmark.maximum) {
			markPosition = ExtentsGetter.GetMaxMarkPosition(referenceTransform.position, transformList, boundType);
		}
		Vector3 activePosition = markPosition;
		
		foreach (Transform nextTransform in transformList) {
			Vector3 newPos;
			Vector3 delta = Vector3.zero;
			if (alignType == Landmark.maximum) {
				if (nextTransform.GetComponent<Renderer>()) {
					delta = -nextTransform.GetComponent<Renderer>().bounds.extents;
				} else if (nextTransform.GetComponent<Collider>()) {
					delta = -nextTransform.GetComponent<Collider>().bounds.extents;
				}
			} else if (alignType == Landmark.minimum) {
				if (nextTransform.GetComponent<Renderer>()) {
					delta = nextTransform.GetComponent<Renderer>().bounds.extents;
				} else if (nextTransform.GetComponent<Collider>()) {
					delta = nextTransform.GetComponent<Collider>().bounds.extents;
				}
			}
			// refers to axisList : None, X, Y, Z, All -> 0, 1, 2, 3, 4
			newPos.x = (axis == Vector3.one || axis == Vector3.right) ? activePosition.x + delta.x : nextTransform.position.x;
			newPos.y = (axis == Vector3.one || axis == Vector3.up) ? activePosition.y + delta.y : nextTransform.position.y;
			newPos.z = (axis == Vector3.one || axis == Vector3.forward) ? activePosition.z + delta.z : nextTransform.position.z;
			nextTransform.position = newPos;
		}
	}

	/// <summary>
	/// Align all transforms rotation with one Transform, or on mini/max values of boundaries
	/// </summary>
	/// <param name="referenceTransform">
	/// A <see cref="Transform"/> which all other transforms will be aligned with, if alignType is "mean"
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	/// <param name="axis">
	/// The axis to align on : <see cref="Vector3.one"/> to align all axis, <see cref="Vector3.right"/> to align on the X axis, <see cref="Vector3.up"/> to align on the Y axis, <see cref="Vector3.forward"/> to align on the Z axis
	/// </param>
	/// <param name="alignType">
	/// Witch type of alignement we do, from the <see cref="Landmark"/> enum
	/// </param>
	public static void AlignRotation(Transform referenceTransform, Transform[] transformList, Vector3 axis, Landmark alignType) {
		
		// Get the rotation from the active selected object
		Vector3 activeRotation = referenceTransform.eulerAngles;
		
		// If alignment is not the mean one, search the min or max oriented object
		Vector3 markRotation = referenceTransform.eulerAngles;
		if (alignType == Landmark.minimum) {
			markRotation = BasicExtents.GetMinMarkRotation(referenceTransform.eulerAngles, transformList);
		} else if (alignType == Landmark.maximum) {
			markRotation = BasicExtents.GetMaxMarkRotation(referenceTransform.eulerAngles, transformList);
		}
		activeRotation = markRotation;
		
		foreach (Transform nextTransform in transformList) {
			Vector3 newRot;
			// Apply the angles changes from the axis to align with
			newRot.x = (axis == Vector3.one || axis == Vector3.right) ? activeRotation.x : nextTransform.eulerAngles.x;
			newRot.y = (axis == Vector3.one || axis == Vector3.up) ? activeRotation.y : nextTransform.eulerAngles.y;
			newRot.z = (axis == Vector3.one || axis == Vector3.forward) ? activeRotation.z : nextTransform.eulerAngles.z;
			nextTransform.rotation = Quaternion.Euler(newRot);
		}
	}

	/// <summary>
	/// Align all transforms local scale with one Transform, or on mini/max values of boundaries
	/// </summary>
	/// <param name="referenceTransform">
	/// A <see cref="Transform"/> which all other transforms will be aligned with, if alignType is "mean"
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	/// <param name="axis">
	/// The axis to align on : <see cref="Vector3.one"/> to align all axis, <see cref="Vector3.right"/> to align on the X axis, <see cref="Vector3.up"/> to align on the Y axis, <see cref="Vector3.forward"/> to align on the Z axis
	/// </param>
	/// <param name="alignType">
	/// Witch type of alignement we do, from the <see cref="Landmark"/> enum
	/// </param>
	public static void AlignScale(Transform referenceTransform, Transform[] transformList, Vector3 axis, Landmark alignType) {
		// Get the local scale from the active selected object
		Vector3 activeScale = referenceTransform.localScale;
		// If alignment is not the mean one, search the min or max positioned object
		Vector3 markScale = referenceTransform.localScale;
		if (alignType == Landmark.minimum) {
			markScale = BasicExtents.GetMinMarkScale(referenceTransform.localScale, transformList);
		} else if (alignType == Landmark.maximum) {
			markScale = BasicExtents.GetMaxMarkScale(referenceTransform.localScale, transformList);
		}
		activeScale = markScale;

		foreach (Transform nextTransform in transformList) {
			Vector3 newScale;
			// Apply the angles changes from the axis to align with
			newScale.x = (axis == Vector3.one || axis == Vector3.right) ? activeScale.x : nextTransform.localScale.x;
			newScale.y = (axis == Vector3.one || axis == Vector3.up) ? activeScale.y : nextTransform.localScale.y;
			newScale.z = (axis == Vector3.one || axis == Vector3.forward) ? activeScale.z : nextTransform.localScale.z;
			nextTransform.localScale = newScale;
		}
	}
	
	// <summary>
	/// Raycast down from the object bounds so it appears to have fallen on the colliders below (from the down to the top)
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	public static void FallOnTerrain(Transform[] transformList) {
		FallOnTerrain(transformList, false, ExtentsGetter.BoundType.Renderer);
	}
	
	// <summary>
	/// Raycast down from the object bounds so it appears to have fallen on the colliders below (from the down to the top)
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	/// <param name="rotateToTerrainAngle">
	/// <see cref="true"/> to rotate <see cref="Transform"/> to the normal from the raycast hit
	/// </param>
	public static void FallOnTerrain(Transform[] transformList, bool rotateToTerrainAngle, ExtentsGetter.BoundType boundType) {
		// List of selected objects, to sort from the min position to the max position
		List<Transform> list = new List<Transform>(transformList);

		// Sort the selected objects from the down to the top, so the lowest 'falls' first
		list.Sort(DistributionManager.ByVector3PositionY);
		
		foreach (Transform fallingTransform in list) {
			FallOnTerrain(fallingTransform, rotateToTerrainAngle, boundType);
		}
	}
	
	// <summary>
	/// Raycast down from the object bounds so it appears to have fallen on the colliders below (from the down to the top)
	/// </summary>
	/// <param name="fallingTransform">
	/// The <see cref="Transform"/> of the object to be aligned
	/// </param>
	public static void FallOnTerrain(Transform fallingTransform) {
		FallOnTerrain(fallingTransform, false, ExtentsGetter.BoundType.Renderer);
	}
	
	// <summary>
	/// Raycast down from the object bounds so it appears to have fallen on the colliders below (from the down to the top)
	/// Beware : due to approximation in Transform values, it's not 100% accurate
	/// </summary>
	/// <param name="fallingTransform">
	/// The <see cref="Transform"/> of the object to be aligned
	/// </param>
	/// <param name="rotateToTerrainAngle">
	/// <see cref="true"/> to rotate <see cref="Transform"/> to the normal from the raycast hit
	/// </param>
	public static void FallOnTerrain(Transform fallingTransform, bool rotateToTerrainAngle, ExtentsGetter.BoundType boundType) {
		Bounds activeObjectBounds = ExtentsGetter.GetHierarchyBounds(fallingTransform.gameObject, boundType);
		// TODO : rewrite so it use a bounding box to get objects inside and then the upper one, then use Collider.ClosestPointOnBounds to get the closest hit point
		
		// If we apply a rotation to the object when falling, make it fall as if it was upside
		Quaternion wasRotation = fallingTransform.rotation;
		if (rotateToTerrainAngle) {
			fallingTransform.rotation = Quaternion.identity;
		}

		float distance = System.Single.PositiveInfinity;
		bool hitSomething = false;
		// The radius is calculated from the bound size, taking the greatest value of width and depth
		float sphereRadius = Mathf.Max(activeObjectBounds.extents.x, activeObjectBounds.extents.z);
		
		// Set the default new position to the current position
		Vector3 newPosition = fallingTransform.position;
		Vector3 normal = fallingTransform.up;

		// New from 2.1 : the Raycast has been replaced by a SphereCastAll method so every single object in the sphere sweep returns a hit
		// SphereCast down to check all hits
		RaycastHit[] hits;
		// TIPS  : the cast can return the current Transform so we must ensure to not take it into account
		hits = Physics.SphereCastAll(activeObjectBounds.center + Vector3.up * sphereRadius, sphereRadius, Vector3.down);
		if (hits.Length > 0) {
			for (int i=0;i<hits.Length;i++) {
				// Somehow, the hits[i].distance is not equal to (raycast.origin - hits[i].point) ?
				float hitDistance = activeObjectBounds.center.y - hits[i].point.y;
				// v2.2 Fix : do not take into account any hit from inside the boundaries (use the y extents)
				if (!hits[i].collider.transform.Equals(fallingTransform) && hitDistance < distance && hitDistance > activeObjectBounds.extents.y) {
					hitSomething = true;
					distance = hitDistance;
					normal = hits[i].normal;
				}
			}
		}

		if (hitSomething) {
			if (distance > 0) {
				// Do not move the object if the distance is 0 or below
				if (rotateToTerrainAngle) {
					// Assume a well oriented mesh (Vector3.up means the up side of the mesh
					Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
					wasRotation = rotation;
				}
				
				// Move to new position : nearest hit distance - half size of the falling object
				newPosition.y = newPosition.y - distance + activeObjectBounds.extents.y;
				fallingTransform.position = newPosition;
			}
			fallingTransform.rotation = wasRotation;
		}
	}
	
	/// <summary>
	/// Replace the position of every Transform by the position of the next one in the list (and the last one by the first one)
	/// </summary>
	/// <param name='transformList'>
	/// Transform list.
	/// </param>
	public static void SwitchPosition(Transform[] transformList) {
		int count = transformList.Length;
		
		if (count <= 1) return;
		
		Vector3 oldPosition = transformList[0].position;
		
		for (int i=0;i<count-1;i++)
			transformList[i].position = transformList[i+1].position;
		
		transformList[count-1].position = oldPosition;
	}
	
	/// <summary>
	/// Replace the rotation of every Transform by the rotation of the next one in the list (and the last one by the first one)
	/// </summary>
	/// <param name='transformList'>
	/// Transform list.
	/// </param>
	public static void SwitchRotation(Transform[] transformList) {
		int count = transformList.Length;
		
		if (count <= 1) return;
		
		Quaternion oldRotation = transformList[0].rotation;
		
		for (int i=0;i<count-1;i++)
			transformList[i].rotation = transformList[i+1].rotation;
		
		transformList[count-1].rotation = oldRotation;
	}
	
	/// <summary>
	/// Replace the scale of every Transform by the scale of the next one in the list (and the last one by the first one)
	/// </summary>
	/// <param name='transformList'>
	/// Transform list.
	/// </param>
	public static void SwitchScale(Transform[] transformList) {
		int count = transformList.Length;
		
		if (count <= 1) return;
		
		Vector3 oldScale = transformList[0].localScale;
		
		for (int i=0;i<count-1;i++)
			transformList[i].localScale = transformList[i+1].localScale;
		
		transformList[count-1].localScale = oldScale;
	}
}
