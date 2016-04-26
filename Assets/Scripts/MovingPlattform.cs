using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlattform : MonoBehaviour
{

    [SerializeField]
    Transform[] points;

    [SerializeField]
    float timeForOneTravel;

    //After the plattform arrives at the last plattform, it will stop moving.
    [SerializeField]
    bool onlyMoveOnce = false;

    [SerializeField]
    float targetRadius = 0.5f;

    new Rigidbody2D rigidbody;
    float plattformSpeed;
    int nextPointToReach;
    //If set to false, the plattform will not move.
    bool shouldMove = true;
    float targetRadiusSqr;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        //Make sure the rigidbody is set to kinematic
        rigidbody.isKinematic = true;

        //Square the targetrad, to avoid root calculation later
        targetRadiusSqr = targetRadius * targetRadius;

        //Disable movement, when less then two points are given
        if (points.Length <= 1)
        {
            shouldMove = false;
            return;
        }

        //Calc the plattform speed
        plattformSpeed = CalcTotalTravelLength() / timeForOneTravel;
    }

    float CalcTotalTravelLength()
    {
        float totalDistance = 0;
        for (int i = 0; i < points.Length - 1; i++)
            totalDistance += Vector3.Distance(points[i].position, points[i + 1].position);
        return totalDistance;
    }

    void FixedUpdate()
    {
        if (!shouldMove)
            return;

        //Calc the direction
        Vector3 targetDir = points[nextPointToReach].position - transform.position;

        //Did we arrive at our target?
        if (targetDir.sqrMagnitude < targetRadiusSqr)
        {
            AdvancePointCycle();
        }

        //We didnt so move on
        targetDir.Normalize();
        rigidbody.velocity = targetDir * plattformSpeed;
    }

    void AdvancePointCycle()
    {
        nextPointToReach++;

        //Finished a complete travel to all points
        if (nextPointToReach == points.Length)
        {
            nextPointToReach = 0;
            if (onlyMoveOnce)
                shouldMove = false;
        }
    }

    void OnFakeCollisionStay2D(ICharacterControllerInput2D iInput)
    {
        iInput.AddRelativeForce(GetVelocity);
    }

    Vector2 GetVelocity ()
    {
        return rigidbody.velocity;
    }
}
