using UnityEngine;
using System.Collections;

public class PlayMonsterAnimation02 : MonoBehaviour {

    public GameObject monster;

    Animator anim;

    // Use this for initialization
    void Start()
    {
        // anim = GetComponent<Animator>();
        anim = monster.GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       if (other.gameObject.CompareTag("Player"))
            {
                //change animator variable
                anim.SetBool("light-01", true);
            }
     }
        
}
