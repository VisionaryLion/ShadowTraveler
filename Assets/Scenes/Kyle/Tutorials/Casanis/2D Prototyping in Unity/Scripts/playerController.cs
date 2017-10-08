using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

    //movement variables
    public float maxSpeed;

    Rigidbody2D myRB; //reference to the rigidbody on the player
    Animator myAnim; //reference to the player's animator
    
    // Use this for initialization
	void Start () {
        myRB = GetComponent<Rigidbody2D>(); //GetComponent looks at the asset the script is attached to for a certain object
        myAnim = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
