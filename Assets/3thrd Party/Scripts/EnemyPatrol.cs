using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

	public float moveSpeed;
	public bool moveRight;

	public Transform wallCheck;
	public float wallCheckRadius;
	public LayerMask whatIsWall;
	private bool hittingWall;

	private bool notAtEdge;
	public Transform edgeCheck;

	public Transform lightCheck;
	public float lightCheckRadius;
	public LayerMask whatIsLight;
	private bool hittingLight;

	private AnimatedPixelPack.Character thePlayer;

	private EnemyPatrol theEnemy;

	//range the enemy looks for the player
	public float playerRange;

	//layer the enemy looks for the player
	public LayerMask playerLayer;

	//if the player is in range or not
	public bool playerInRange;

	//delay for enemy fleeing from light
	//public float fleeTimer;

	//range the enemy looks for the light
	public float lightRange;

	//if light is in range or not
	public bool lightInRange;

		// Use this for initialization
	void Start () {
		thePlayer = FindObjectOfType<AnimatedPixelPack.Character> ();

		theEnemy = FindObjectOfType<EnemyPatrol> ();
	
	}
	
	// Update is called once per frame
	void Update () {

		//detect when hitting the wall
		hittingWall = Physics2D.OverlapCircle (wallCheck.position, wallCheckRadius, whatIsWall);

		//detect when hitting light
		hittingLight = Physics2D.OverlapCircle (lightCheck.position, lightCheckRadius, whatIsLight);

		//detect when not at the edge
		notAtEdge = Physics2D.OverlapCircle (edgeCheck.position, wallCheckRadius, whatIsWall);

		//detec when the player is in range
		playerInRange = Physics2D.OverlapCircle (transform.position, playerRange, playerLayer);

		//detec when light is in range
		lightInRange = Physics2D.OverlapCircle (transform.position, lightRange, whatIsLight);

		//default enemy behaviour
		if (!playerInRange) 
		{
			moveSpeed = 2;

			//checks if the enemy is hitting the wall or is not at an edge
			if (hittingWall || !notAtEdge || hittingLight)
				moveRight = !moveRight;

			//if moving right then positive move speed and if not then negative move speed for left
			if (moveRight) {
				transform.localScale = new Vector3 (1f, 1f, 1f);
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
			} else {
				transform.localScale = new Vector3 (-1f, 1f, 1f);
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (-moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
			}
		}

		//enemy behaviour if player in range
		if (playerInRange) 
		{
			//StartCoroutine ("AttackPlayerCo");

			//find x position of player
			var playerObject = thePlayer;
			var playerPos = playerObject.transform.position.x;

			//find x position of enemy
			var enemyObject = theEnemy;
			var enemyPos = enemyObject.transform.position.x;

			//check if enemy is going right
			if (enemyPos < playerPos) {

				//if hitting a wall, at an edge, or hitting light then stop moving
				if (hittingWall || !notAtEdge || hittingLight) {
					//transform.localScale = new Vector3 (-1f, 1f, 1f);
					//GetComponent<Rigidbody2D> ().velocity = new Vector2 (-moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
					moveSpeed = 0;
				} 

				//if no obstacles then go right
				if (!hittingWall || notAtEdge || !hittingLight) {
					transform.localScale = new Vector3 (1f, 1f, 1f);
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
					moveSpeed = 3;
				}

				//if light in range go left
				if (lightInRange) {
					transform.localScale = new Vector3 (-1f, 1f, 1f);
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (-moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
					moveSpeed = 3;
				}
			} 

			//check if enemy is going left
			if (enemyPos > playerPos) {

				//if hitting a wall, at an edge, or hitting light then stop moving
				if (hittingWall || !notAtEdge || hittingLight) {
					//transform.localScale = new Vector3 (1f, 1f, 1f);
					//GetComponent<Rigidbody2D> ().velocity = new Vector2 (moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
					moveSpeed = 0;
				} 

				//if no obstacles then go left
				if (!hittingWall || notAtEdge || !hittingLight) {
					transform.localScale = new Vector3 (-1f, 1f, 1f);
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (-moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
					moveSpeed = 3;
				}

				//if light in range go right
				if (lightInRange) {
					transform.localScale = new Vector3 (1f, 1f, 1f);
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
					moveSpeed = 3;
				}
			}

		}

	}

	//public IEnumerator AttackPlayerCo()
	//{
		
	//}

	void OnDrawGizmosSelected () {

		//draws spehere around enemy showing his range to find the player
		Gizmos.DrawSphere (transform.position, playerRange);

		//draws spehere around enemy showing his range to find light
		Gizmos.DrawSphere (transform.position, lightRange);
	}
}