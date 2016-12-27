using UnityEngine;
using System.Collections;
using CC2D;

namespace Entity
{
    public class HumanMovementEntity : IMovementEntity
    {
        [SerializeField]
        CC2DMotor cC2DMotor;

        public override CC2DMotor CC2DMotor { get { return cC2DMotor; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            cC2DMotor = GetComponentInChildren<CC2DMotor>();
        }
#endif
    }
}
