using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

    //movement variables
    public float maxSpeed;

    Rigidbody2D myRB; //reference to the rigidbody on the player
    Animator myAnim; //reference to the player's animator
    bool facingRight; //variable for if the player is facing to the right is true or not
    
    // Use this for initialization
	void Start () {
        myRB = GetComponent<Rigidbody2D>(); //GetComponent looks at the asset the script is attached to for a certain object
        myAnim = GetComponent<Animator>();

        facingRight = true; //starts the character facing to the right
		
	}
	
	// Update is called once per frame (no matter how long the frame took) VS FixedUpdate is called after a specific amount of time all the time (it's exact)
	void FixedUpdate () {
        float move = Input.GetAxis("Horizontal"); //GetAxis is between -1 and 1, GetAxisRaw is -1, 0, or 1 //makes a float variable assigned to the Horizontal axis which are used by pressing the A and D keys or the left and righ arrow keys
        myAnim.SetFloat("speed", Mathf.Abs(move)); //sets the value of speed as an absoulte value for move

        myRB.velocity = new Vector2(move * maxSpeed, myRB.velocity.y); //for the x value it multiplies the value for move by the maxSpeed set on the player & doesn't change the y value

        //if the player is pressing the D key and isn't facing right (facing left) else if the player is pressing the A key and facing right
        if (move > 0 && !facingRight) {
            flip(); //call the flip function
        } else if (move < 0 && facingRight) { 
            flip(); //call the flip function
        }
	}

    void flip() {
        facingRight = !facingRight; //reverses whichever way the player was facing
        Vector3 theScale = transform.localScale; //applies the transfrom values (x,y,z) from the player to the localScale and puts it to theScale
        theScale.x *= -1; //makes the x value of the scale negative or positive depending on its current value
        transform.localScale = theScale; //sets the new value back onto the transform value on the player
    }
}
