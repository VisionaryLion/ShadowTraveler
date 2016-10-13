using UnityEngine;
using System.Collections;
using CC2D;
using System;

public class PathSegment : IPathSegment
{
    Vector2 goal;
    Vector2 start; // only for debuging!
    float xMin;
    float xMax;
    int moveDir;
    float distance;
    MovementInput input;

    public PathSegment (Vector2 start, Vector2 goal, float distance)
    {
        if (start.x > goal.x)
        {
            xMin = goal.x;
            xMax = start.x;
            moveDir = -1;
        }
        else
        {
            xMin = start.x;
            xMax = goal.x;
            moveDir = 1;
        }
        xMin -= 0.1f;
        xMax += 0.1f;
        this.goal = goal;
        this.start = start;
        this.distance = distance;
    }

    public override bool SetsInitialVelocity
    {
        get
        {
            return false;
        }
    }

    public override float TraverseDistance
    {
        get
        {
            return distance;
        }
    }

    public override Vector2 GetInitialVelocity()
    {
        throw new NotImplementedException();
    }

    public override MovementInput GetMovementInput()
    {
        return input;
    }

    public override bool IsOnTrack(Vector2 position)
    {
        if (position.x < xMin || position.x > xMax)
            return false;
        return true;
    }

    public override bool ReachedTarget(Vector2 position)
    {
        if ((position - goal).sqrMagnitude <= 0.01f)
            return true;
        return false;
    }

    public override void InitTravers()
    {
        input = new MovementInput() { horizontal = moveDir, horizontalRaw = moveDir };
    }

    public override void StopTravers()
    {
        input = null;
    }

    public override void Visualize()
    {
        Debug.DrawLine(start, goal, Color.green);
    }
}
