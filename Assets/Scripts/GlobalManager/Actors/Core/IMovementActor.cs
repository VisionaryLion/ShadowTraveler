using UnityEngine;
using System.Collections;
using CC2D;

namespace Actors
{
    public abstract class IMovementActor : SimpleMovementActor
    {
        public abstract CC2DMotor CC2DMotor { get; }
    }
}
