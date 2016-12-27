using UnityEngine;
using System.Collections;

namespace Entity
{
    public class ElevatorEntity : Entity
    {
        [SerializeField]
        new Rigidbody2D rigidbody2D;
        [SerializeField]
        Collider2D trigger;
        [SerializeField]
        ElevatorPlatform elevatorPlatform;

        #region public
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } }
        public Collider2D Trigger { get { return trigger; } }
        public ElevatorPlatform ElevatorPlatform { get { return elevatorPlatform; } }
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            rigidbody2D = LoadComponent<Rigidbody2D>(rigidbody2D);
            trigger = LoadComponent<Collider2D>("Trigger", trigger);
            elevatorPlatform = LoadComponent<ElevatorPlatform>("ElevatorPlatform", elevatorPlatform);

            //Setup some script vars automatically.
            this.tag = "MovingPlatform"; //Built-in-Tag can't go wrong.
        }
#endif
    }
}
