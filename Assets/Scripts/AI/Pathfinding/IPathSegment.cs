using UnityEngine;
using System.Collections;
using System;
using CC2D;

[Serializable]
public abstract class IPathSegment {
    public abstract bool SetsInitialVelocity { get; }
    public abstract float TraverseDistance { get; }

    public abstract void InitTravers();
    public abstract void StopTravers();
    public abstract Vector2 GetInitialVelocity();
    public abstract MovementInput GetMovementInput();
    public abstract bool IsOnTrack(Vector2 position);
    public abstract bool ReachedTarget(Vector2 position);
    public abstract void Visualize();
}
