using UnityEngine;
using SurfaceTypeUser;
using System.Collections;

/*
Author: Oribow
*/
namespace Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class ForceDamageReceptor : MonoBehaviour
    {
        public IHealth health;
        public Vector3 directionDamageMultiplier;
        public bool enableSurfaceMultipliers;
        public float[] surfaceMultiplier;
        public float minImpactMag;

        void OnCollisionEnter (Collision collision)
        {
            Vector3 resultingHealthImpact = collision.relativeVelocity;
            resultingHealthImpact.x *= directionDamageMultiplier.x;
            resultingHealthImpact.y *= directionDamageMultiplier.y;
            resultingHealthImpact.z *= directionDamageMultiplier.z;
            float damage = resultingHealthImpact.magnitude;
            if (enableSurfaceMultipliers)
            {
                damage *= surfaceMultiplier[(int)SurfaceTypeManager.GetSurfaceType(collision.collider, collision.contacts[0].point)];
            }
            if (damage > minImpactMag)
            {
                Debug.Log(name + " recieved " + collision.impulse + " hit, resulting in " + damage + "total damage.");
                health.ChangeHealth(new BasicDamageInfo(IDamageInfo.DamageTyp.Collision, -damage));
            }
        }
    }
}
