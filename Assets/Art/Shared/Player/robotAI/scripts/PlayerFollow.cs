﻿using UnityEngine;
using System.Collections;

public class PlayerFollow : MonoBehaviour 
{
	public float xMargin = 1f;		// Distance in the x axis the player can move before the friend follows.
	public float yMargin = 1f;		// Distance in the y axis the player can move before the friend follows.
	public float xSmooth = 8f;		// How smoothly the friend catches up with it's target movement in the x axis.
	public float ySmooth = 8f;		// How smoothly the friend catches up with it's target movement in the y axis.
    [SerializeField]
    float RotationSpeed = 15;
    
    private Transform player;		// Reference to the player's transform location, specifically the gameobject "RobotAILocation"
    //private Transform Guntip;       // Reference to the gun's tip
    public Vector2 Guntip;
    public GameObject Gunshot;

    public float fireRate = 0.2f;
    public float bulletSpeed = 1.5f;
    public GameObject Bullet;
    public static float damage = 10;

    private float lastShot = 0.0f;



	void Awake ()
	{
        // Setting up the reference location for friend to follow.
        player = GameObject.FindGameObjectWithTag("RobotAILocation").transform;
    }
    
	void FixedUpdate ()
	{
		TrackPlayer();
        RotateMe();

        Guntip = new Vector2(Gunshot.transform.position.x, Gunshot.transform.position.y);

        if (Input.GetButton("RobotFire")) {
                Shoot();
        }
	}
	
	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;

		// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
		targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);

		// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
		targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);
        
		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}

   
    void RotateMe()
    {
        transform.Rotate(0.0f, 0.0f, -Input.GetAxis("RobotHorizontal") * RotationSpeed);  
    }

    void Shoot()
    {
        //Debug.LogError("Shoot");
        //check the rate of fire
        if (Time.time > fireRate + lastShot)
        {
            //Instantiate a bullet
            GameObject bulletClone = Instantiate(Bullet, Guntip, Quaternion.identity) as GameObject;
            Rigidbody2D clonerb = bulletClone.GetComponent<Rigidbody2D>();
            clonerb.AddRelativeForce(transform.TransformDirection(new Vector2((Mathf.Cos(transform.rotation.z * Mathf.Deg2Rad) * bulletSpeed), (Mathf.Sin(transform.rotation.z * Mathf.Deg2Rad) * bulletSpeed))), ForceMode2D.Impulse);
            lastShot = Time.time;
        }
    }
}
