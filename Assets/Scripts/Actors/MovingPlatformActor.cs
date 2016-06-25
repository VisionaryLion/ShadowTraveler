using UnityEngine;

namespace Actors
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MovingPlattform))] //Requires for it self: PositionHolder2D, Rigidbody2D
    [RequireComponent(typeof(Collider2D))]
    public class MovingPlatformActor : Actor
    {
        [HideInInspector]
        [SerializeField]
        MovingPlattform movingPlattform;
        [HideInInspector]
        [SerializeField]
        PositionHolder2D positionHolder2D;
        [HideInInspector]
        [SerializeField]
        new Rigidbody2D rigidbody2D;
        [HideInInspector]

        #region public
        public MovingPlattform MovingPlattform { get { return movingPlattform; } }
        public PositionHolder2D PositionHolder2D { get { return positionHolder2D; } }
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } }
        #endregion

#if UNITY_EDITOR
        protected override void Awake()
        {
            if (_executOnce) //already executed this script. No need for setting things up
                return;
            base.Awake();

            //Load components
            movingPlattform = GetComponent<MovingPlattform>();
            positionHolder2D = GetComponent<PositionHolder2D>();
            rigidbody2D = GetComponent<Rigidbody2D>();

            //Setup some script vars automatically.
            rigidbody2D.isKinematic = true;

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminderShort("Tag"));
            _executOnce = true;
        }
#endif
    }
}