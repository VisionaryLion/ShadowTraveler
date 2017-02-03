using UnityEngine;
using System.Collections;

/// <summary>
/// Collider Extents : child of BasicExtents for Collider specific bounds
/// </summary>
public class ColliderExtents : BasicExtents {
	
	/// <summary>
	/// Retrieve the boundaries from the collider extents of the game object and its children
	/// </summary>
	/// <param name="activeGO">
	/// The parent <see cref="GameObject"/> to start with
	/// </param>
	/// <returns>
	/// The bounds from the collider extents and all its children.
	/// </returns>
	public static Bounds GetHierarchyBounds(GameObject activeGO) {
		Bounds totalBounds;
		Collider[] colliders = activeGO.GetComponentsInChildren<Collider>();

		if (activeGO.GetComponent<Collider>())
			totalBounds = GetColliderBounds(activeGO.GetComponent<Collider>());
		else if (colliders.Length > 0)
			totalBounds = GetColliderBounds(colliders[0]);
		else 
			totalBounds = new Bounds(activeGO.transform.position, Vector3.zero);
		foreach (Collider collider in colliders) {
			totalBounds.Encapsulate(GetColliderBounds(collider));
		}
		return totalBounds;
	}
	
	
	/// <summary>
	/// Retrieve the minimum coordinates for each axis from a transform list
	/// </summary>
	/// <param name="referencePosition">
	/// The first position to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve position from
	/// </param>
	/// <returns>
	/// The minimum extents in a <see cref="Vector3"/>
	/// </returns>
	public new static Vector3 GetMinMarkPosition(Vector3 referencePosition, Transform[] transformList) {
		Vector3 markPosition = referencePosition;
		foreach (Transform nextTransform in transformList) {
			// Do not take size into account when making a distribution
			Bounds bounds = new Bounds(nextTransform.position, Vector3.zero);
			
			if (nextTransform.GetComponent<Collider>()) {
				bounds = GetColliderBounds(nextTransform.GetComponent<Collider>());
			}
			
			if (nextTransform.position.x - bounds.extents.x < markPosition.x)
				markPosition.x = nextTransform.position.x - bounds.extents.x;
			if (nextTransform.position.y - bounds.extents.y < markPosition.y)
				markPosition.y = nextTransform.position.y - bounds.extents.y;
			if (nextTransform.position.z - bounds.extents.z < markPosition.z)
				markPosition.z = nextTransform.position.z - bounds.extents.z;
		}
		return markPosition;
	}
	
	
	/// <summary>
	/// Retrieve the maximum coordinates for each axis from a transform list
	/// </summary>
	/// <param name="referencePosition">
	/// The first position to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve position from
	/// </param>
	/// <returns>
	/// The maximum extents in a <see cref="Vector3"/>
	/// </returns>
	public new static Vector3 GetMaxMarkPosition(Vector3 referencePosition, Transform[] transformList) {
		Vector3 markPosition = referencePosition;
		// Boundaries are and can not always be used in those cases
		foreach (Transform nextTransform in transformList) {
			// Do not take size into account when making a distribution
			Bounds bounds = new Bounds(nextTransform.position, Vector3.zero);
			
			if (nextTransform.GetComponent<Collider>()) {
				bounds = GetColliderBounds(nextTransform.GetComponent<Collider>());
			}
			
			if (nextTransform.position.x + bounds.extents.x > markPosition.x)
				markPosition.x = nextTransform.position.x + bounds.extents.x;
			if (nextTransform.position.y + bounds.extents.y > markPosition.y)
				markPosition.y = nextTransform.position.y + bounds.extents.y;
			if (nextTransform.position.z + bounds.extents.z > markPosition.z)
				markPosition.z = nextTransform.position.z + bounds.extents.z;
		}
		return markPosition;
	}

	/// <summary>
	/// Gets the right collider bounds : if it's a capsule collider, the unity.bounds returns a bounding box of the capsule radius, not using the height, so it's wrong and it must be otherwise
	/// </summary>
	/// <returns>
	/// The collider bounds, or new bounds taking into account the height of a capsule collider
	/// </returns>
	/// <param name='myCollider'>
	/// My collider.
	/// </param>
	static Bounds GetColliderBounds(Collider myCollider) {
		Bounds bounds;
		/*
		 // The Capsule Collider bug has been solved in Unity 3.5.x
		if (myCollider.GetType().Equals(typeof(CapsuleCollider))) { 
			CapsuleCollider myCapsule = (myCollider as CapsuleCollider);
			bounds = new Bounds(myCollider.bounds.center, Vector3.Scale(myCollider.transform.localScale, new Vector3(myCapsule.radius, myCapsule.height, myCapsule.radius)));
		}
		else if (myCollider.GetType().Equals(typeof(CharacterController))) { 
			CharacterController myCapsule = (myCollider as CharacterController);
			bounds = new Bounds(myCollider.bounds.center, Vector3.Scale(myCollider.transform.localScale, new Vector3(myCapsule.radius, myCapsule.height, myCapsule.radius)));
		} 
		else {
		// */
			bounds = myCollider.bounds;
		//}
		return bounds;
	}
}
