using UnityEditor;
using UnityEngine;
using System.Collections;

public class SwitchPressurePlate : MonoBehaviour
{
    public enum TargetType
    {
        Door, Light, LaserWall, SwitchLines, GravityVent
    }

    public TargetType targetType;

    Animator animator;

    [Tooltip("Try to use a color that matches the ActivationObject.")]
    public Color ButtonColor;
    public GameObject Button;
    [Tooltip("This is the tags for the GameObject that activates the switch.")]
    public string ActivationObject;
    [Tooltip("This is the Door that will be turned on or off by the switch.")]
    public Doorway01[] TargetDoors;
    [Tooltip("These are the Lights that will be turned on or off by the switch.")]
    public Lightbulb[] TargetLights;
    [Tooltip("These are the laserWalls that will be turned on or off by the switch.")]
    public LaserWallSolid[] TargetLaserWalls;
    [Tooltip("These are the Switch lines that will be turned on or off by the switch.")]
    public SwitchLineColor[] TargetSwitchLines;

    //private 


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
                    foreach (Doorway01 door in TargetDoors)
                    {
                        door.HasPower = !door.HasPower;
                    }
                    break;
                case TargetType.Light:
                    //set Light variables
                    foreach (Lightbulb light in TargetLights)
                    {
                        light.HasPower = !light.HasPower;
                    }
                    break;
                case TargetType.LaserWall:
                    //set LaserWall variables
                    foreach (LaserWallSolid LaserBeam in TargetLaserWalls)
                    {
                        LaserBeam.HasPower = !LaserBeam.HasPower;
                    }
                    break;
                case TargetType.SwitchLines:
                    //set switchLines variables
                    foreach (SwitchLineColor SwitchLine in TargetSwitchLines)
                    {
                        SwitchLine.HasPower = !SwitchLine.HasPower;
                    }
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
                    foreach (Doorway01 door in TargetDoors)
                    {
                        door.HasPower = !door.HasPower;
                    }
                    break;
                case TargetType.Light:
                    //set Light variables
                    foreach (Lightbulb light in TargetLights)
                    {
                        light.HasPower = !light.HasPower;
                    }
                    break;
                case TargetType.LaserWall:
                    //set LaserWall variables
                    foreach (LaserWallSolid LaserBeam in TargetLaserWalls)
                    {
                        LaserBeam.HasPower = !LaserBeam.HasPower;
                    }
                    break;
                case TargetType.SwitchLines:
                    //set switchLines variables
                    foreach (SwitchLineColor SwitchLine in TargetSwitchLines)
                    {
                        SwitchLine.HasPower = !SwitchLine.HasPower;
                    }
                    break;
                case TargetType.GravityVent:
                    //set GravityVent variables
                    break;
            }
        }
    }
}
