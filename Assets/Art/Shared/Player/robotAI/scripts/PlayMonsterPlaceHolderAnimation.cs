using UnityEngine;
using System.Collections;

public class PlayMonsterPlaceHolderAnimation : MonoBehaviour
{

    public GameObject monster;
    //public GameObject guard01;
    //public GameObject guard02;
    private bool anim1 = false;
    Animator anim;

    // Use this for initialization
    void Start()
    {
        // anim = GetComponent<Animator>();
        anim = monster.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
       
           if (other.gameObject.CompareTag("Player"))
            {
                //change animator variable
                anim1 = true;
                anim.SetBool("switch-01", true);
            }
    }
}
