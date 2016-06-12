using UnityEngine;
using System.Collections;

namespace FakePhysics
{
    [RequireComponent(typeof(CharacterController2D))]
    public class RigidbodyInteractor : MonoBehaviour
    {

        CharacterController2D controller;
        new Rigidbody2D rigidbody;
        Collider2D collider;

        void Awake()
        {
            controller = GetComponent<CharacterController2D>();
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
        }

        void Start()
        {
            controller.onControllerCollidedEvent += Controller_onControllerCollidedEvent;
        }

        private void Controller_onControllerCollidedEvent(RaycastHit2D obj)
        {

            Rigidbody2D oRi = obj.collider.GetComponent<Rigidbody2D>();
            if (oRi == null)
                return;
            Debug.Log("Collided with rigidbody");

            // Calculate relative velocity
            Vector2 rv = oRi.velocity - controller.velocity;

            // Calculate relative velocity in terms of the normal direction
            float velAlongNormal = Vector2.Dot(rv, obj.normal);

            // Do not resolve if velocities are separating
            if (velAlongNormal > 0)
                return;

            // Calculate restitution
            float e = Mathf.Min((obj.collider.sharedMaterial == null) ? 0 : obj.collider.sharedMaterial.bounciness, (collider.sharedMaterial == null) ? 0 : collider.sharedMaterial.bounciness);

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j /= 1 / rigidbody.mass + 1 / oRi.mass;

            // Apply impulse
            Vector2 impulse = j * obj.normal;
            oRi.AddForceAtPosition(impulse, obj.point, ForceMode2D.Impulse);
            controller.moveSilent(-impulse * 1/rigidbody.mass, false);
        }
    }
}