using UnityEngine;
using System.Collections;
using CC2D;
using Pathfinding2D;
using System;

public class JumpSegment : IPathSegment
{

    public const float TimeOutFudgeSeconds = 3;

    float xVel;
    float jumpForce;
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    float distance;
    Vector2 goal;
    Vector2 start; //only for debug

    public JumpSegment(JumpLink link)
    {
        goal = link.endPoint;
        xVel = link.xVel;
        jumpForce = link.jumpForce;
        xMin = link.xMin - 0.1f;
        xMax = link.xMax + 0.1f;
        yMin = link.yMin - 0.1f;
        yMax = link.yMax + 0.1f;
        distance = link.xMax - link.xMin;
        start = link.startPoint;
    }

    public override bool SetsInitialVelocity
    {
        get
        {
            return true;
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
        return new Vector2(xVel, jumpForce);
    }

    public override MovementInput GetMovementInput()
    {
        return MovementInput.EmptyInput;
    }

    public override bool IsOnTrack(Vector2 position)
    {
        if (position.x < xMin || position.x > xMax || position.y < yMin || position.y > yMax)
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

    }

    public override void StopTravers()
    {

    }

    public override void Visualize()
    {
       
        Debug.DrawLine(start, goal, Color.blue);
    }
}
