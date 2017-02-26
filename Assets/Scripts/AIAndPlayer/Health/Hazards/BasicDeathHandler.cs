using UnityEngine;
using UnityEngine.Events;
/*
Author: Oribow
*/
namespace Combat
{
    public class BasicDeathHandler : MonoBehaviour
    {
        public IHealth healthComponent; 
        public UnityEvent deathActions;

        void Start()
        {
            healthComponent.OnDeath += HealthComponent_OnDeath;
        }

        private void HealthComponent_OnDeath(object sender, System.EventArgs e)
        {
            deathActions.Invoke();
        }
    }
}
