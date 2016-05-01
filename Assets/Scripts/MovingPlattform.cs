using UnityEngine;

namespace FakePhysics
{
    [RequireComponent(typeof(CharacterController2D))]
    public class MovingPlattform : MonoBehaviour
    {

        [SerializeField]
        Transform[] points;

        [SerializeField]
        float timeForOneTravel;

        //After the plattform arrives at the last plattform, it will stop moving.
        [SerializeField]
        bool onlyMoveOnce = false;

        [SerializeField]
        float targetRadius = 0.5f;

        CharacterController2D charController;
        float plattformSpeed;
        int nextPointToReach;
        //If set to false, the plattform will not move.
        bool shouldMove = true;
        float targetRadiusSqr;
        Vector3 velocity;

        void Awake()
        {
            charController = GetComponent<CharacterController2D>();
            charController.onControllerCollidedEvent += CharController_onControllerCollidedEvent;

            //Square the targetrad, to avoid root calculation later
            targetRadiusSqr = targetRadius * targetRadius;

            //Disable movement, when less then two points are given
            if (points.Length <= 1)
            {
                shouldMove = false;
                return;
            }

            //Calc the plattform speed
            plattformSpeed = CalcTotalTravelLength() / timeForOneTravel;
        }

        private void CharController_onControllerCollidedEvent(RaycastHit2D obj)
        {
            IManagedCharController2D iInput = obj.collider.GetComponent<IManagedCharController2D>();
            if (iInput != null)
            {
                iInput.AddForce(velocity);
            }
        }

        float CalcTotalTravelLength()
        {
            float totalDistance = 0;
            for (int i = 0; i < points.Length - 1; i++)
                totalDistance += Vector3.Distance(points[i].position, points[i + 1].position);
            return totalDistance;
        }

        void FixedUpdate()
        {
            if (!shouldMove)
                return;

            //Calc the direction
            velocity = points[nextPointToReach].position - transform.position;

            //Did we arrive at our target?
            if (velocity.sqrMagnitude < targetRadiusSqr)
            {
                AdvancePointCycle();
            }

            //We didnt so move on
            velocity.Normalize();
            charController.move(velocity * plattformSpeed * Time.fixedDeltaTime, false);
        }

        void AdvancePointCycle()
        {
            nextPointToReach++;

            //Finished a complete travel to all points
            if (nextPointToReach == points.Length)
            {
                nextPointToReach = 0;
                if (onlyMoveOnce)
                    shouldMove = false;
            }
        }

        void OnFakeCollisionStay2D(IManagedCharController2D iInput)
        {
            iInput.AddPlattformVelocity(()=> { return charController.velocity /2; });
        }
    }
}
