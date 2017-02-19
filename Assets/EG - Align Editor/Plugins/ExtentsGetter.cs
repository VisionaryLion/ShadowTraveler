// by Equilibre Games http://www.equilibregames.com
// Lead: Frederic Rolland-Porche
// listed in unity asset store since version 1.2
//
// Post your feature request on Unity3D forum / Assets and Asset Store / Align Editor !
//
using UnityEngine;
using System.Collections;

/// <summary>
/// Extents getter : use BasicExtents and children classes to manage maximum and minimum values for position, rotation, scale and bounds from a Transform array
/// </summary>
public class ExtentsGetter {

	public enum BoundType {
		Renderer,
		Collider,
		None
	}
	
	/// <summary>
	/// Retrieve the boundaries from the renderer extents of the game object and its children
	/// </summary>
	/// <param name="activeGO">
	/// The parent <see cref="GameObject"/> to start with
	/// </param>
	/// <returns>
	/// The bounds from the renderer extents and all its children.
	/// </returns>
	public static Bounds GetHierarchyBounds(GameObject activeGO, BoundType type) {
		switch (type) {
		case BoundType.Collider : return ColliderExtents.GetHierarchyBounds(activeGO);
		case BoundType.Renderer : return RendererExtents.GetHierarchyBounds(activeGO);
		default: return new Bounds(Vector3.zero, Vector3.zero);
		}
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
	public static Vector3 GetMinMarkPosition(Vector3 referencePosition, Transform[] transformList, BoundType type) {
		switch (type) {
		case BoundType.Collider : return ColliderExtents.GetMinMarkPosition(referencePosition, transformList);
		case BoundType.Renderer : return RendererExtents.GetMinMarkPosition(referencePosition, transformList);
		default: return BasicExtents.GetMinMarkPosition(referencePosition, transformList);
		}
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
	public static Vector3 GetMaxMarkPosition(Vector3 referencePosition, Transform[] transformList, BoundType type) {
		switch (type) {
		case BoundType.Collider : return ColliderExtents.GetMaxMarkPosition(referencePosition, transformList);
		case BoundType.Renderer : return RendererExtents.GetMaxMarkPosition(referencePosition, transformList);
		default: return BasicExtents.GetMaxMarkPosition(referencePosition, transformList);
		}
	}
}
