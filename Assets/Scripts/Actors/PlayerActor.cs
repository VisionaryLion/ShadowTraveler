using UnityEngine;
using CC2D;
using Combat;


//Movement
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(CC2DMotor))]
[RequireComponent(typeof(HumanInput))]

//Health
[RequireComponent(typeof(BasicHealth))]
[RequireComponent(typeof(BasicDamageReceptor))]

[ExecuteInEditMode]
public class PlayerActor : MonoBehaviour {

#if UNITY_EDITOR
    
    void Awake()
    {
        for(int i = 0; i < 8; i++)
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this); //Move our self up to the top!
        this.tag = "Player"; //Built-in-Tag can't go wrong.
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<BasicDamageReceptor>().BaseHealth = GetComponent<BasicHealth>();
        Debug.LogWarning("You still need to: [General] Configure the \"Layer\"");
        Debug.LogWarning("You still need to: [CC2DMotor] Assign the \"Sprite Root\"");
        Debug.LogWarning("You still need to: [CC2DMotor] Define \"Climbable Tag\"");
        Debug.LogWarning("You still need to: [CC2DMotor] Configure the layer mask \"Wall Slideable\"");
        Debug.LogWarning("You still need to: [Animator] Assign a \"Controller\"");
        Debug.LogWarning("You still need to: [Character Controller 2D] Configure the layer mask  \"Platform Mask\"");
        Debug.LogWarning("You still need to: [Character Controller 2D] Configure the layer mask  \"Trigger Mask\"");
        Debug.LogWarning("You still need to: [Character Controller 2D] Configure the layer mask  \"One Way Platform Mask\"");
    }

#endif
}
