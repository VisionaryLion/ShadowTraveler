using UnityEngine;
using System.Collections;
using System;
using CC2D;

[Serializable]
public abstract class IOffNodeLink
{
    public int targetNodeIndex;
    public int targetVertIndex;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float traversCosts;
}
