using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
	
	public Transform center;
	public float speed = 5f;
	public Vector2 mouseSpeed = Vector2.one * 5;
	
	private Vector2 inputPosition;
	private bool wasDown;
	public float currentSpeed = 0;
	private bool positiveRotation = true;
	
	// Use this for initialization
	void Start () {
		wasDown = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			inputPosition = Input.mousePosition;
			wasDown = true; 
			currentSpeed = 0;
		} else if (Input.GetMouseButton(0)) {
			if (wasDown) {
				float xDelta = Input.mousePosition.x - inputPosition.x;
				float yDelta = inputPosition.y - Input.mousePosition.y;
				positiveRotation = (xDelta > 0);
				Vector3 pos = transform.position;
				transform.Translate(Vector3.up * yDelta * mouseSpeed.y * Time.deltaTime);
				pos.y = Mathf.Clamp(transform.position.y, 20, 180);
				if (pos.y != transform.position.y)
					transform.position = pos;
				
				transform.RotateAround(center.position, Vector3.up, mouseSpeed.x * xDelta * Time.deltaTime);
				
				transform.LookAt(center.position);
				inputPosition = Input.mousePosition;
			}
		}
		else {
			wasDown = false;
			// Rotate anyway
			currentSpeed = Mathf.Lerp(currentSpeed, speed * (positiveRotation ? 1 : -1), 0.1f * Time.deltaTime);
			transform.RotateAround(center.position, Vector3.up, currentSpeed * Time.deltaTime);
			transform.LookAt(center.position);
		}
	}
}
