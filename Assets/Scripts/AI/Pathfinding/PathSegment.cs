using UnityEngine;
using System.Collections;
using CC2D;
using System;

public class PathSegment : IPathSegment
{
    public const float TimeOutFudgeSeconds = 3;

    Vector2 goal;
    Vector2 start; // only for debuging!
    float xMin;
    float xMax;
    int moveDir;
    float timeOut;

    public PathSegment (Vector2 start, Vector2 goal, float timeOut)
    {
        if (start.x > goal.x)
        {
            xMin = goal.x - 0.1f;
            xMax = start.x + 1;
            moveDir = -1;
        }
        else
        {
            xMin = start.x - 1;
            xMax = goal.x + 0.1f;
            moveDir = 1;
        }
        this.goal = goal;
        this.start = start;
        this.timeOut = timeOut + TimeOutFudgeSeconds;
    }

    public override float TimeOut
    {
        get
        {
            return timeOut;
        }
    }

    public override void UpdateMovementInput(MovementInput input)
    {
        input.horizontal = moveDir;
        input.horizontalRaw = moveDir;
    }

    public override bool IsOnTrack(Vector2 position)
    {
        if (position.x < xMin || position.x > xMax)
            return false;
        return true;
    }

    public override bool ReachedTarget(Vector2 position)
    {
        if ((position - goal).sqrMagnitude <= 0.05f)
            return true;
        return false;
    }

    public override void InitTravers(CC2DThightAIMotor motor)
    {
        
    }

    public override void StopTravers(CC2DThightAIMotor motor)
    {
        
    }

    public override void Visualize()
    {
        Debug.DrawLine(start, goal, Color.green);
    }
}
