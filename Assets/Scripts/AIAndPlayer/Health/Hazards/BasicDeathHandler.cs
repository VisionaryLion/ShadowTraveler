using UnityEngine;
/*
Author: Oribow
*/
namespace Combat.Hazard
{
    public class BasicDeathHandler : MonoBehaviour
    {
        public IHealth healthComponent;
        public bool destroyObject = true;
        public GameObject deathPrefab;
        public float destroyDeathPrefabAfterTime;

        void Start()
        {
            healthComponent.OnDeath += HealthComponent_OnDeath;
        }

        private void HealthComponent_OnDeath(object sender, System.EventArgs e)
        {
            if (deathPrefab != null)
            {
                GameObject clone = Instantiate(deathPrefab, transform.position, Quaternion.identity) as GameObject;
                clone.name = deathPrefab.name;
                if(destroyDeathPrefabAfterTime > 0)
                Destroy(clone, destroyDeathPrefabAfterTime);
            }
            if (destroyObject)
                Destroy(gameObject);
        }
    }
}
