using UnityEngine;
using System.Collections;

/// <summary>
/// Renderer Extents : child of BasicExtents for Renderer specific bounds
/// </summary>
public class RendererExtents : BasicExtents {
	
	/// <summary>
	/// Retrieve the boundaries from the renderer extents of the game object and its children
	/// </summary>
	/// <param name="activeGO">
	/// The parent <see cref="GameObject"/> to start with
	/// </param>
	/// <returns>
	/// The bounds from the renderer extents and all its children.
	/// </returns>
	public static Bounds GetHierarchyBounds(GameObject activeGO) {
		Bounds totalBounds;
		Renderer[] renderers = activeGO.GetComponentsInChildren<Renderer>();
		
		if (activeGO.GetComponent<Renderer>())
			totalBounds = activeGO.GetComponent<Renderer>().bounds;
		else if (renderers.Length > 0)
			totalBounds = (renderers[0]).bounds;
		else 
			totalBounds = new Bounds(activeGO.transform.position, Vector3.zero);
		foreach (Renderer render in renderers) {
			totalBounds.Encapsulate(render.bounds);
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
			
			if (nextTransform.GetComponent<Renderer>()) {
				bounds = nextTransform.GetComponent<Renderer>().bounds;
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
			
			if (nextTransform.GetComponent<Renderer>()) {
				bounds = nextTransform.GetComponent<Renderer>().bounds;
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
}
