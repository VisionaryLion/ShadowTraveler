using UnityEngine;
using System.Collections;

public class TargetPractice : MonoBehaviour {

    public float HP = 1;
    public ParticleSystem TargetExplode;

    private Transform player;
    public float TargetSpeed = 2f;
    private bool FollowPlayer = false;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate () {
        //check if the object is hit
        player = GameObject.FindGameObjectWithTag("Player").transform;
        TargetMovePosistion();
	}

    //function moves target to center of it's parent game object (0, 0, 0)
    void TargetMovePosistion()
    {
        if (FollowPlayer == false)
        {
            //transform.localPosition = new Vector3(0, 0, 0);
            transform.position = Vector2.MoveTowards(transform.position, transform.position, TargetSpeed * Time.deltaTime);
        }
        else
        {
            //moves toward player, having issues where targets bottleneck in corners
            //...may need a seperate environment collider for smooth target movement 
            transform.position = Vector2.MoveTowards(transform.position, player.position, TargetSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Player")))
        {
            FollowPlayer = true;
            //transform.position = Vector2.MoveTowards(transform.position, player.position, TargetSpeed * Time.deltaTime);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Player")))
        {
            FollowPlayer = false;
            //transform.localPosition = new Vector3(0, 0, 0);
            //transform.position = Vector2.MoveTowards(transform.position, 0, TargetSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //check if collision is with bullet
        if ((coll.gameObject.CompareTag("Bullet"))){
            //play particle effect on hitting anything
            GameObject expl = Instantiate(TargetExplode, transform.position, Quaternion.identity).gameObject;
            Destroy(expl, 1); // delete the particle after 3 seconds
            //subtract health
            HP = HP - PlayerFollow.damage; //gets damage from the PlayerFollow script

            //destroy the object after its HP reaches 0    
            if (HP <= 0)
            {
                //lower the TotalEnemies count
                TargetManager.TotalEnemies = TargetManager.TotalEnemies - 1;
                Destroy(gameObject);
            }          
        }
    }
}
