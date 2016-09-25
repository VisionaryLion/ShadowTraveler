using UnityEngine;
using System.Collections;
using Actors;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField, AssignActorAutomaticly, HideInInspector]
    ElevatorActor actor;

    [SerializeField]
    Transform[] levels;
    [SerializeField]
    int currentLevel;
    [SerializeField]
    AnimationCurve speed;
    [SerializeField]
    float goalRad;
    [SerializeField]
    float speedMultiplier;
    [SerializeField]
    InteractiveInputDefintion elevatorUp;
    [SerializeField]
    InteractiveInputDefintion elevatorDown;

    float moveStartTime;
    int direction;

    void Awake()
    {
        elevatorUp.onButtonDown = OnUpButtonDown;
        elevatorDown.onButtonDown = OnDownButtonDown;
    }

    void OnUpButtonDown(BasicEntityActor entityActor)
    {
        if (currentLevel < levels.Length - 1)
        {
            moveStartTime = Time.time;
            actor.Collider2D.enabled = false;
            direction = 1;
        }
        else
        {
            Debug.Log("Shouldnt have the option to go up!");
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorUp);
        }
    }

    void OnDownButtonDown(BasicEntityActor entityActor)
    {
        if (currentLevel > 0)
        {
            moveStartTime = Time.time;
            actor.Collider2D.enabled = false;
            direction = -1;
        }
        else
        {
            Debug.Log("Shouldnt have the option to go down!");
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorDown);
        }
    }

    void Update()
    {
        if (direction != 0)
        {
            actor.Rigidbody2D.velocity = Vector2.up * direction * speed.Evaluate(Time.time - moveStartTime) * speedMultiplier * Time.deltaTime;
            //Debug.Log(speed.Evaluate(Time.time - moveStartTime));
            if ((levels[currentLevel + direction].position.y - transform.position.y) * direction < goalRad)
            {
                
                actor.Rigidbody2D.velocity = Vector2.zero;
                currentLevel += direction;
                transform.position = new Vector2(transform.position.x, levels[currentLevel].position.y);
                direction = 0;
                actor.Collider2D.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        BasicEntityActor actor = col.GetComponent<BasicEntityActor>();
        if (currentLevel < levels.Length -1)
            actor.InteractiveInputHandler.AddInputListener(elevatorUp);
        if (currentLevel > 0)
            actor.InteractiveInputHandler.AddInputListener(elevatorDown);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        BasicEntityActor actor = col.GetComponent<BasicEntityActor>();
        actor.InteractiveInputHandler.RemoveInputListener(elevatorUp);
        actor.InteractiveInputHandler.RemoveInputListener(elevatorDown);
    }
}
