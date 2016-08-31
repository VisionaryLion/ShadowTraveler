using UnityEngine;

/*
Author: Oribow
*/
namespace Combat.Hazard
{
    public class BasicExplosiv : MonoBehaviour
    {
        public IHealth healthComponent;
        public IDamageInfo.DamageTyp damageTyp;
        public float damage = 40;
        public float physicalForce = 40;
        public float range = 10;
        public LayerMask damageMask;
        public bool debugRange = true;

        void Start()
        {
            healthComponent.OnDeath += HealthComponent_OnDeath;
        }

        private void HealthComponent_OnDeath(object sender, System.EventArgs e)
        {
            healthComponent.IsSilent = true; //Without that we are entering a infinity loop!!
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, damageMask);
            foreach (Collider col in colliders)
            {
                IDamageReciever reciever = col.GetComponent<IDamageReciever>();
                if (reciever != null)
                    reciever.TakeDamage(new BasicDamageInfo(damageTyp, -Mathf.Max(damage - Vector3.Distance(col.transform.position, transform.position), 0)));

                Rigidbody rig = col.GetComponent<Rigidbody>();
                if (rig != null)
                    rig.AddExplosionForce(physicalForce, transform.position, range);
            }
            healthComponent.IsSilent = false;
        }

        void OnDrawGizmos()
        {
            if (!debugRange)
                return;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
