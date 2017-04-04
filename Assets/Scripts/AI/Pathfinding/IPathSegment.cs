using UnityEngine;
using System.Collections;
using System;
using CC2D;

[Serializable]
public abstract class IPathSegment {
    public abstract float TimeOut { get; }

    public abstract float StartSpeed(CC2DAIMotor motor);
    public abstract void InitTravers(CC2DAIMotor motor, IPathSegment nextSeg);
    public abstract void StopTravers(CC2DAIMotor motor);
    public abstract void UpdateMovementInput(MovementInput input, CC2DAIMotor motor);
    public abstract bool IsOnTrack(Vector2 position);
    public abstract bool ReachedTarget(Vector2 position);
    public abstract void Visualize();
}
