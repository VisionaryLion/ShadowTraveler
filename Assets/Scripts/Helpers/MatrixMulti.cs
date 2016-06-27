using UnityEngine;
using System.Collections;

public class MatrixMulti : MonoBehaviour {
    public Transform t1;
    public Transform t2;

	// Use this for initialization
	void Start () {
	Matrix4x4 combination = t1.worldToLocalMatrix * t2.worldToLocalMatrix;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
