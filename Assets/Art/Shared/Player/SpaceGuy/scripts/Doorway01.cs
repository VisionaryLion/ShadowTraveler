using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Doorway01 : MonoBehaviour {

    //scene to go to
    public bool HasPower = true; //add static later
    public GameObject doorLight;
    public Color lightOnColor;
    public Color lightOffColor;
    public Text text;
    public string DestinationName;

    Animator animator;
    //Text text;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        text.text = DestinationName;

        if (HasPower == true)
        {
            //light is green
            doorLight.GetComponent<SpriteRenderer>().color = lightOnColor;
        }
        else
        {
            //light is red
            doorLight.GetComponent<SpriteRenderer>().color = lightOffColor;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (HasPower == true)
        {
            //light is green
            doorLight.GetComponent<SpriteRenderer>().color = lightOnColor;
        }
        else
        {
            //light is red
            doorLight.GetComponent<SpriteRenderer>().color = lightOffColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Player")))
        {
            if (HasPower == true)
            {
                animator.SetBool("DoorActive", true);
                //light is green
                //doorLight.GetComponent<SpriteRenderer>().color = lightOnColor;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Player")))
        {
            if (HasPower == true)
            {
                animator.SetBool("DoorActive", false);
                //light is red
                //doorLight.GetComponent<SpriteRenderer>().color = lightOffColor;
            }
        }
    }

    void ActivateDoor()
    {
        HasPower = true;
    }

    void DeactivateDoor()
    {
        HasPower = false;
    }

}
