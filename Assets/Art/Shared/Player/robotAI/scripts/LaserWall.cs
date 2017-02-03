using UnityEngine;
using System.Collections;

public class LaserWall : MonoBehaviour {

    public GameObject Base;
    public float laserYScale = 38;          //distance the laser travels  
    public float laserDownTime = 3f;        //amount of time the laser is active
    public float laserUpTime = 3f;          //amount of time the laser is not active
    public float animationSpeed = 1.25f;    //higher is faster, but could effect laser distance
    public static bool laserActive = true;  //this could be used to disable laser when the power is off


    // Use this for initialization
    void Start()
    {
        StartCoroutine(LaserPulse());
    }
	
    IEnumerator LaserPulse()
    {
        while (laserActive == true)
        {
            //define the local scale variable
            Vector3 theScale = transform.localScale;

            //laser is retracted
            Base.GetComponent<Collider2D>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            while (theScale.y > 1)
            {
                theScale.y = theScale.y - animationSpeed;
                transform.localScale = theScale;
                yield return new WaitForSeconds(.0005f);
            }

            yield return new WaitForSeconds(laserUpTime);

            //laser is active
            Base.GetComponent<Collider2D>().enabled = true;
            this.GetComponent<Collider2D>().enabled = true;
            while (theScale.y < laserYScale)
            {
                theScale.y = theScale.y + animationSpeed;
                transform.localScale = theScale;
                yield return new WaitForSeconds(.0005f);
            }

            yield return new WaitForSeconds(laserDownTime);
        }
    }
    
}
