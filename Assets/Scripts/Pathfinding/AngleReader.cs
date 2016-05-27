using UnityEngine;
using System.Collections;

public class AngleReader : MonoBehaviour {
    public Vector2 dir;
    public Vector2 dir2;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Angle = "+Vector2.Angle(dir, dir2));
	}
}
