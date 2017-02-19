using UnityEngine;
using System.Collections;

/// <summary>
/// Basic Extents : manage maximum and minimum values for position, rotation, scale from a Transform array
/// </summary>
public class BasicExtents {
	
	/// <summary>
	/// Retrieve the maximum coordinates for each axis from a transform list
	/// </summary>
	/// <param name="referencePosition">
	/// The first position to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve position from
	/// </param>
	/// <param name="useBounds">
	/// A <see cref="System.Boolean"/> that tells if the extents must be used
	/// </param>
	/// <returns>
	/// The maximum extents in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMaxMarkPosition(Vector3 referencePosition, Transform[] transformList) {
		Vector3 markPosition = referencePosition;
		// Boundaries are and can not always be used in those cases
		foreach (Transform nextTransform in transformList) {
			// Do not take size into account when making a distribution
			Bounds bounds = new Bounds(nextTransform.position, Vector3.zero);
			
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
	/// Retrieve the minimum coordinates for each axis from a transform list
	/// </summary>
	/// <param name="referencePosition">
	/// The first position to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve position from
	/// </param>
	/// <param name="useBounds">
	/// A <see cref="System.Boolean"/> that tells if the extents must be used
	/// </param>
	/// <returns>
	/// The minimum extents in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMinMarkPosition(Vector3 referencePosition, Transform[] transformList) {
		Vector3 markPosition = referencePosition;
		foreach (Transform nextTransform in transformList) {
			// Do not take size into account when making a distribution
			Bounds bounds = new Bounds(nextTransform.position, Vector3.zero);
			
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
	/// Retrieve the maximum euler angles for each axis from a transform list
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve euler angles from
	/// </param>
	/// <returns>
	/// The minimum euler angles in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMaxMarkRotation(Transform[] transformList) {
		if (transformList.Length > 0)
			return GetMaxMarkRotation(transformList[0].eulerAngles, transformList);
		return Vector3.zero;
	}


	/// <summary>
	/// Retrieve the maximum euler angles for each axis from a transform list
	/// </summary>
	/// <param name="referenceEulerAngles">
	/// The first rotation to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve euler angles from
	/// </param>
	/// <returns>
	/// The maximum euler angles in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMaxMarkRotation(Vector3 referenceEulerAngles, Transform[] transformList) {
		Vector3 markRotation = referenceEulerAngles;
		// Selection.activeTransform.eulerAngles
		foreach (Transform nextTransform in transformList) {
			if (nextTransform.eulerAngles.x > markRotation.x)
				markRotation.x = nextTransform.eulerAngles.x;
			if (nextTransform.eulerAngles.y > markRotation.y)
				markRotation.y = nextTransform.eulerAngles.y;
			if (nextTransform.eulerAngles.z > markRotation.z)
				markRotation.z = nextTransform.eulerAngles.z;
		}
		return markRotation;
	}

	/// <summary>
	/// Retrieve the minimum euler angles for each axis from a transform list
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve euler angles from
	/// </param>
	/// <returns>
	/// The minimum euler angles in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMinMarkRotation(Transform[] transformList) {
		if (transformList.Length > 0)
			return GetMinMarkRotation(transformList[0].eulerAngles, transformList);
		return Vector3.zero;
	}

	/// <summary>
	/// Retrieve the minimum euler angles for each axis from a transform list
	/// </summary>
	/// <param name="referenceEulerAngles">
	/// The first rotation to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve euler angles from
	/// </param>
	/// <returns>
	/// The minimum euler angles in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMinMarkRotation(Vector3 referenceEulerAngles, Transform[] transformList) {
		Vector3 markRotation = referenceEulerAngles;
		foreach (Transform nextTransform in transformList) {
			if (nextTransform.eulerAngles.x < markRotation.x)
				markRotation.x = nextTransform.eulerAngles.x;
			if (nextTransform.eulerAngles.y < markRotation.y)
				markRotation.y = nextTransform.eulerAngles.y;
			if (nextTransform.eulerAngles.z < markRotation.z)
				markRotation.z = nextTransform.eulerAngles.z;
		}
		return markRotation;
	}

	/// <summary>
	/// Retrieve the maximum local scale for each axis from a transform list
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve local scales from
	/// </param>
	/// <returns>
	/// The maximum local scales in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMaxMarkScale(Transform[] transformList) {
		if (transformList.Length > 0)
			return GetMaxMarkScale(transformList[0].localScale, transformList);
		return Vector3.zero;
	}

	/// <summary>
	/// Retrieve the maximum local scale for each axis from a transform list
	/// </summary>
	/// <param name="referenceScale">
	/// The first local scale to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve local scales from
	/// </param>
	/// <returns>
	/// The maximum local scales in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMaxMarkScale(Vector3 referenceScale, Transform[] transformList) {
		Vector3 markScale = referenceScale;
		
		foreach (Transform nextTransform in transformList) {
			if (nextTransform.localScale.x > markScale.x)
				markScale.x = nextTransform.localScale.x;
			if (nextTransform.localScale.y > markScale.y)
				markScale.y = nextTransform.localScale.y;
			if (nextTransform.localScale.z > markScale.z)
				markScale.z = nextTransform.localScale.z;
		}
		return markScale;
	}

	/// <summary>
	/// Retrieve the minimum local scale for each axis from a transform list
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve local scales from
	/// </param>
	/// <returns>
	/// The minimum local scales in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMinMarkScale(Transform[] transformList) {
		if (transformList.Length > 0)
			return GetMinMarkScale(transformList[0].localScale, transformList);
		return Vector3.zero;
	}

	/// <summary>
	/// Retrieve the minimum local scale for each axis from a transform list
	/// </summary>
	/// <param name="referenceScale">
	/// The first local scale to be used as a reference, in a <see cref="Vector3"/>
	/// </param>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> to retrieve local scales from
	/// </param>
	/// <returns>
	/// The minimum local scales in a <see cref="Vector3"/>
	/// </returns>
	public static Vector3 GetMinMarkScale(Vector3 referenceScale, Transform[] transformList) {
		Vector3 markScale = referenceScale;
		foreach (Transform nextTransform in transformList) {
			if (nextTransform.localScale.x < markScale.x)
				markScale.x = nextTransform.localScale.x;
			if (nextTransform.localScale.y < markScale.y)
				markScale.y = nextTransform.localScale.y;
			if (nextTransform.localScale.z < markScale.z)
				markScale.z = nextTransform.localScale.z;
		}
		return markScale;
	}

}
