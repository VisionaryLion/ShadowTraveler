using UnityEngine;
using System.Collections;

namespace FakePhysics
{
    public class ForceField : MonoBehaviour
    {

        public Vector2 force;
        public bool linearLess;
        public Vector2 anker;
        public float length;

        void OnTriggerEnter2D(Collider2D other)
        {
            IManagedCharController2D iInput = other.GetComponent<IManagedCharController2D>();
            if (iInput != null)
            {
                iInput.AddConstantVelocity(force);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            IManagedCharController2D iInput = other.GetComponent<IManagedCharController2D>();
            if (iInput != null)
            {
                iInput.RemoveConstantVelocity(force);
            }
        }

        void OnDrawGizmos()
        {
            if (linearLess)
            {
                Gizmos.DrawRay(anker, force * length);
            }
        }
    }
}
