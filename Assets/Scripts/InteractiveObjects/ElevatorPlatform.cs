using UnityEngine;
using System.Collections;
using Entities;

public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField, AssignEntityAutomaticly, HideInInspector]
    ElevatorEntity actor;

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
    InteractiveInputDefinition elevatorUp;
    [SerializeField]
    InteractiveInputDefinition elevatorDown;
    public string elevatorStartEventF = "";
    public string elevatorEndEventF = "";

    float moveStartTime;
    int direction;

    void Awake()
    {
        elevatorUp.onButtonDown = OnUpButtonDown;
        elevatorDown.onButtonDown = OnDownButtonDown;
    }

    void OnUpButtonDown(ActingEntity entityActor)
    {
        if (currentLevel < levels.Length - 1)
        {
            StartElevatorMovingSound();
             moveStartTime = Time.time;
            actor.Trigger.enabled = false;
            direction = 1;
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorUp);
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorDown);
        }
        else
        {
            Debug.Log("Shouldnt have the option to go up!");
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorUp);
        }
    }

    void OnDownButtonDown(ActingEntity entityActor)
    {
        if (currentLevel > 0)
        {
            StartElevatorMovingSound();
            moveStartTime = Time.time;
            actor.Trigger.enabled = false;
            direction = -1;
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorUp);
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorDown);
        }
        else
        {
            Debug.Log("Shouldnt have the option to go down!");
            entityActor.InteractiveInputHandler.RemoveInputListener(elevatorDown);
        }
    }

    void StartElevatorMovingSound()
    {


    }

    void EndElevatorMovingSound()
    {

    }

    void Update()
    {
        if (direction != 0)
        {
            actor.Rigidbody2D.velocity = Vector2.up * direction * speed.Evaluate(Time.time - moveStartTime) * speedMultiplier * Time.deltaTime;

            //Debug.Log(speed.Evaluate(Time.time - moveStartTime));
            if ((levels[currentLevel + direction].position.y - transform.position.y) * direction < goalRad)
            {
                EndElevatorMovingSound();
                actor.Rigidbody2D.velocity = Vector2.zero;
                currentLevel += direction;
                transform.position = new Vector2(transform.position.x, levels[currentLevel].position.y);
                direction = 0;
                actor.Trigger.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        ActingEntity actor = col.GetComponent<ActingEntity>();
        if (actor == null)
            return;
        if (currentLevel < levels.Length -1)
            actor.InteractiveInputHandler.AddInputListener(elevatorUp);
        if (currentLevel > 0)
            actor.InteractiveInputHandler.AddInputListener(elevatorDown);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        ActingEntity actor = col.GetComponent<ActingEntity>();
        if (actor == null)
            return;
        actor.InteractiveInputHandler.RemoveInputListener(elevatorUp);
        actor.InteractiveInputHandler.RemoveInputListener(elevatorDown);
    }
}
