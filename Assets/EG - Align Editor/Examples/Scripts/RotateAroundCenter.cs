using UnityEngine;
using System.Collections;

public class RotateAroundCenter : MonoBehaviour {
	
	public Transform center;
	public float speed = 5f;
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(center.position, transform.up, speed * Time.deltaTime);
		//transform.LookAt(center.position);
	}
}
