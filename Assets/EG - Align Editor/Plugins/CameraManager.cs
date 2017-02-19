using UnityEngine;
using System.Collections;

/// <summary>
/// Camera manager : Functions that works with Camera and Screen
/// </summary>
public class CameraManager {
	
	/// <summary>
	/// Align all transforms so they face the current camera
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	public static void OrientToCamera(Transform[] transformList, Camera camera) {
		
		foreach (Transform nextTransform in transformList) {
			nextTransform.LookAt(camera.transform.position);
		}
	}
	
	/// <summary>
	/// Align all transforms so they are in the screen center
	/// </summary>
	/// <param name="transformList">
	/// The <see cref="Transform[]"/> of all objects to be aligned
	/// </param>
	public static void CenterInScreen(Transform[] transformList, Camera camera) {
		
		// Get a ray from the center of the camera
		Ray screenRay = camera.ScreenPointToRay(new Vector3(camera.pixelWidth/2, camera.pixelHeight/2, 0));
		float distance;
		
		foreach (Transform nextTransform in transformList) {
			// Create a plane from the object coordinates and the camera (as a normal)
			Plane objectPlane = new Plane(screenRay.direction, nextTransform.position);
			// Cast the ray from the camera to the object plane
			if (objectPlane.Raycast(screenRay, out distance)) {
				nextTransform.position = screenRay.GetPoint(distance);
			}
		}
	}
}
