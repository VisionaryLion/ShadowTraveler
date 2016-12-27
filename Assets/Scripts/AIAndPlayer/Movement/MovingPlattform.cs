using UnityEngine;
using Entities;

[RequireComponent(typeof(PositionHolder2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlattform : MonoBehaviour
{
    enum MovemenType
    {
        OneShot,
        Loop,
        PingPong
    }
    [AssignEntityAutomaticly]
    public MovingPlatformEntity actor;
    [SerializeField]
    float timeForOneTravel = 20;
    [SerializeField]
    MovemenType movemenType;
    [SerializeField]
    float targetRadius = 0.5f;

    float plattformSpeed;
    int nextPointToReach;
    //If set to false, the platform will not move.
    bool shouldMove = true;
    bool backwards;
    float targetRadiusSqr;
    Vector3 velocity;

    void Awake()
    {
        //Square the targetrad, to avoid root calculation later
        targetRadiusSqr = targetRadius * targetRadius;

        //Calc the platform speed
        plattformSpeed = CalcTotalTravelLength() / timeForOneTravel;
    }

    float CalcTotalTravelLength()
    {
        float totalDistance = 0;
        for (int i = 0; i < actor.PositionHolder2D.positions.Count - 1; i++)
            totalDistance += Vector3.Distance(actor.PositionHolder2D.positions[i], actor.PositionHolder2D.positions[i + 1]);
        return totalDistance;
    }

    void FixedUpdate()
    {
        if (!shouldMove)
            return;

        //Calc the direction
        velocity = (Vector3)actor.PositionHolder2D.positions[nextPointToReach] - transform.position;

        //Did we arrive at our target?
        if (velocity.sqrMagnitude < targetRadiusSqr)
        {
            AdvancePointCycle();
        }

        //We didn't, so move on
        velocity.Normalize();
        actor.Rigidbody2D.velocity = velocity * plattformSpeed;
    }

    void AdvancePointCycle()
    {
        if (backwards)
            nextPointToReach--;
        else
            nextPointToReach++;

        //Finished a complete travel to all points
        if (nextPointToReach == actor.PositionHolder2D.positions.Count)
        {
            if (movemenType == MovemenType.PingPong)
            {
                backwards = true;
                nextPointToReach = actor.PositionHolder2D.positions.Count - 2;
            }
            else if (movemenType == MovemenType.OneShot)
                shouldMove = false;
            else
                nextPointToReach = 0;
        }
        else if (nextPointToReach == -1)
        {
            nextPointToReach = 1;
            backwards = false;
        }
    }
}
