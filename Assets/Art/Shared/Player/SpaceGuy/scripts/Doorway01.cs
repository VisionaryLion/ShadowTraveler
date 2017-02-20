using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Doorway01 : MonoBehaviour {

    //add destination scene and player position
    public bool HasPower = true;
    public GameObject doorLight;
    public Color lightOnColor;
    public Color lightOffColor;
    public Text text;
    public string DestinationName;

    public AudioClip DoorOpenSFX;
    public AudioClip DoorCloseSFX;
    public float doorVol = 1;

    AudioSource source;
    Animator animator;


	void Start () {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        text.text = DestinationName;
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
            }
        }
    }

    void playDoorOpenSFX()
    {
        source.PlayOneShot(DoorOpenSFX, doorVol);
    }

    void playDoorCloseSFX()
    {
        source.PlayOneShot(DoorCloseSFX, doorVol);
    }

}
