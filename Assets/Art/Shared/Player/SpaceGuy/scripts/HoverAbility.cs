using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class HoverAbility : MonoBehaviour
{

    private float HoverTime = 0;
    private bool isHovering = false;

    [Tooltip(".6 in the y value keeps the player straight on the y-axis, no lift")]
    public Vector2 HoverHeight = new Vector2(0f, .6f);

    //TO ADD: link HoverAbility to character controller so player can't jump while hovering
    
    //private bool canHover = true;
    [Tooltip("How long to wait before regen begins. Lower is faster.")]
    public float regenDelay = 1.1f;  //time to wait before regen starts
    [Tooltip("Determines how long the hover will last.")]
    public float HoverDuration = 1.2f;
    [Tooltip("Determines how fast the hover will regernerate. Higher is faster.")]
    public float regenRate = .05f;
    public Image HoverUIbar;
        

    //Every 'regenDelay' seconds the player regains 'regenRate' fuel
    //PROBLEM: The player can spam the Joystick1Button4 and cause the fuel to regenerate faster
    //
    void Update()
    {
        if (Input.GetKey(KeyCode.Joystick1Button4) && (HoverDuration > HoverTime)) //&& (canHover = true)
        {
            isHovering = true;
            Hover();
        }

        //----------------------add variable to only activate recharge if the player is on the ground
        if (Input.GetKeyUp(KeyCode.Joystick1Button4)) //&& (HoverDuration < time)
        {
            isHovering = false;
            StartCoroutine(RegainFuelOverTime()); //Starts the coroutine 
        }
        //Hoverbar fill
        //We can replace and improve the UI image later
        HoverUIbar.fillAmount = (1 - HoverTime);        
    }

        private IEnumerator RegainFuelOverTime()
        {
            //canHover = false;
            while (HoverTime > 0)
            {
                yield return new WaitForSeconds(regenDelay);
                ResetTimer();
            }
            //canHover = true;
        }

        void Hover()
        {
            if (HoverDuration > HoverTime)
            {
                HoverTime += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
                GetComponent<Rigidbody2D>().velocity = HoverHeight;  //generates hover  -- fixed 'y' velocity 
            }
        }

        void ResetTimer()
        {
            //subtract regenRate from time until time reaches zero
            if (HoverTime >= 0)
            {
                HoverTime = HoverTime - regenRate;
            }
            else if (HoverTime < 0)
            {
                HoverTime = 0;
            }
        }
}

