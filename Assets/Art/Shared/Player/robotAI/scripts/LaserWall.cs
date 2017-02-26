using UnityEngine;
using System.Collections;

public class LaserWall : MonoBehaviour {

    public GameObject Base;
    [Tooltip("Distance the laser travels")]
    public float laserYScale = 38;          //distance the laser travels 
    [Tooltip("Amount of time the laser is active (extended)")]
    public float laserDownTime = 3f;        //amount of time the laser is active
    [Tooltip("Amount of time the laser is retracted")]
    public float laserUpTime = 3f;          //amount of time the laser is not active
    [Tooltip("Higher is faster, but large numbers could effect laser distance")]
    public float animationSpeed = 1.25f;    //higher is faster, but could effect laser distance
    public float animationWaitTime = 0.005f;
    public bool LaserWallHasPower = true;   //this could be used to disable laser when the power is off

    public AudioClip LaserActivateSFX;
    public AudioClip LaserDeactivateSFX;
    public float LaserSFXVol = 0.4f;

    AudioSource source;

    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(LaserPulse());
    }
    
    IEnumerator LaserPulse()
    {

        while (LaserWallHasPower == true)
        {
            //define the local scale variable
            Vector3 theScale = transform.localScale;

            //laser is retracted
            Base.GetComponent<Collider2D>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            source.PlayOneShot(LaserDeactivateSFX, LaserSFXVol);
            while (theScale.y > 1)
            {
                theScale.y = theScale.y - animationSpeed;
                transform.localScale = theScale;
                yield return new WaitForSeconds(animationWaitTime);
            }

            yield return new WaitForSeconds(laserUpTime);

            //laser is active
            Base.GetComponent<Collider2D>().enabled = true;
            this.GetComponent<Collider2D>().enabled = true;
            source.PlayOneShot(LaserActivateSFX, LaserSFXVol);
            while (theScale.y < laserYScale)
            {
                theScale.y = theScale.y + animationSpeed;
                transform.localScale = theScale;
                yield return new WaitForSeconds(animationWaitTime);
            }

            yield return new WaitForSeconds(laserDownTime);
        }      
    }
}
