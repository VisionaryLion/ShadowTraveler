using UnityEngine;
using System.Collections;

public class RotateMe : MonoBehaviour {

    public float rotateSpeed = 7;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Rotate the object around its local X axis at 1 degree per second
        //transform.Rotate(Vector3.right * Time.deltaTime);
        transform.Rotate(0, 0, 10 * rotateSpeed);
    }
}
