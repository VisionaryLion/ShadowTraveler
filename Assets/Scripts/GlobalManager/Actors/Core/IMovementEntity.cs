using UnityEngine;
using System.Collections;
using CC2D;

namespace Entities
{
    public abstract class IMovementEntity : SimpleMovingEntity
    {
        public abstract CC2DMotor CC2DMotor { get; }
    }
}
