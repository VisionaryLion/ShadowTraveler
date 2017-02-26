using UnityEditor;
using UnityEngine;
using System.Collections;

public class SwitchPressurePlate : MonoBehaviour
{
    public enum TargetType
    {
        Door, Light, LaserWall, GravityVent
    }

    public TargetType targetType;

    Animator animator;

    [Tooltip("Try to use a color that matches the ActivationObject.")]
    public Color ButtonColor;
    public GameObject Button;
    [Tooltip("This is the tag for the GameObject that activates the switch.")]
    public string ActivationObject;
    [Tooltip("This is the Door that will be turned on or off by the switch.")]
    public Doorway01 TargetDoor;
    [Tooltip("This is the Lights that will be turned on or off by the switch.")]
    public Lightbulb[] TargetLights;
    [Tooltip("This is the LaserWall that will be turned on or off by the switch.")]
    public LaserWall TargetLaserWall;

    private 
    

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        Button.GetComponent<SpriteRenderer>().color = ButtonColor;
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(ActivationObject))
        {
            //GetComponent<
           animator.SetBool("SwitchTouched", true);
            switch (targetType)
            {
                case TargetType.Door:
                    //set door variables
                    TargetDoor.HasPower = !TargetDoor.HasPower;
                    break;

                case TargetType.Light:
                    //set Light variables
                    foreach (Lightbulb light in TargetLights)
                    {
                        light.ToogleStatus(true);
                    }
                    break;
                case TargetType.LaserWall:
                    //set LaserWall variables
                    //TargetLaserWall.LaserWallHasPower = !TargetLaserWall.LaserWallHasPower;
                    break;
                case TargetType.GravityVent:
                    //set GravityVent variables
                    break;
            }
        }
    }
    

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(ActivationObject))
        {
            animator.SetBool("SwitchTouched", false);
            switch (targetType)
            {
                case TargetType.Door:
                    //set door variables
                    TargetDoor.HasPower = !TargetDoor.HasPower;
                    break;
                case TargetType.Light:
                    //set Light variables
                    foreach (Lightbulb light in TargetLights)
                    {
                        light.ToogleStatus(false);
                    }
                    break;
                case TargetType.LaserWall:
                    //set LaserWall variables
                    //TargetLaserWall.LaserWallHasPower = !TargetLaserWall.LaserWallHasPower;
                    break;
                case TargetType.GravityVent:
                    //set GravityVent variables
                    break;
            }
        }
    }
}
