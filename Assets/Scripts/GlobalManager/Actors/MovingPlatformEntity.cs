using UnityEngine;

namespace Entities
{
    public class MovingPlatformEntity : Entity
    {
        [SerializeField]
        MovingPlattform movingPlattform;
        [SerializeField]
        PositionHolder2D positionHolder2D;
        [SerializeField]
        new Rigidbody2D rigidbody2D;

        #region public
        public MovingPlattform MovingPlattform { get { return movingPlattform; } }
        public PositionHolder2D PositionHolder2D { get { return positionHolder2D; } }
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } }
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            //Load components
            movingPlattform = LoadComponent<MovingPlattform>(movingPlattform);
            positionHolder2D = LoadComponent<PositionHolder2D>(positionHolder2D);
            rigidbody2D = LoadComponent<Rigidbody2D>(rigidbody2D);

            //Setup some script vars automatically.
            rigidbody2D.isKinematic = true;

            //Print some custom reminder.
            Debug.LogWarning(GenerateSetUpReminderShort("Tag"));
        }
#endif
    }
}