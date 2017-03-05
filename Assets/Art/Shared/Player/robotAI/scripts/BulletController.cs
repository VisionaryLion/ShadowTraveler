using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public ParticleSystem BulletHit;
    public int bounceCount = 2;
    private int HitCounter = 0;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //play particle effect on hitting anything
        GameObject expl = Instantiate(BulletHit, transform.position, Quaternion.identity).gameObject;
        Destroy(expl, 1); // delete the particle after 3 seconds
        //add to hit counter
        HitCounter = HitCounter + 1;
        //destroy the object after x collisions    
        if (HitCounter >= bounceCount)
        {
            Destroy(gameObject);
        }
    }
}
