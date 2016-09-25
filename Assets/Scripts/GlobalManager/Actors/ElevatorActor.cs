using UnityEngine;
using System.Collections;

namespace Actors
{
    public class ElevatorActor : Actor
    {
        [SerializeField]
        new Rigidbody2D rigidbody2D;
        [SerializeField]
        new Collider2D collider2D;
        [SerializeField]
        ElevatorPlatform elevatorPlatform;

        #region public
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } }
        public Collider2D Collider2D { get { return collider2D; } }
        public ElevatorPlatform ElevatorPlatform { get { return elevatorPlatform; } }
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            rigidbody2D = GetComponentInChildren<Rigidbody2D>();
            collider2D = GetComponentInChildren<Collider2D>();
            elevatorPlatform = GetComponentInChildren<ElevatorPlatform>();

            //Setup some script vars automatically.
            this.tag = "MovingPlatform"; //Built-in-Tag can't go wrong.
        }
#endif
    }
}
