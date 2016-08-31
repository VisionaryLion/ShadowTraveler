using UnityEngine;
using Actors;
using System.Collections;

namespace Combat
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [AssignActorAutomaticly]
        PlayerActor actor;

        void Start()
        {
            actor.BasicHealth.OnDeath += HealthComponent_OnDeath;
        }

        private void HealthComponent_OnDeath(object sender, IDamageInfo info)
        {
           
        }
    }
}
