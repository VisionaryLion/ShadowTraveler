using UnityEngine;
using System.Collections;

public class PlayMonsterAnimation03 : MonoBehaviour {

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
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Box"))
        {
            //change animator variable
            anim.SetBool("switch-02", true);
        }
    }

}
