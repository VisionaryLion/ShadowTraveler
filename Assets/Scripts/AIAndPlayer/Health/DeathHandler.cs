using UnityEngine;
using Entity;
using System.Collections;
using Manager;

namespace Combat
{
    public class DeathHandler : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [AssignEntityAutomaticly]
        ActingEntity actor;

        GameStateEntity gameStateActor;

        void Start()
        {
            actor.IHealth.OnDeath += HealthComponent_OnDeath;
            gameStateActor = EntityDatabase.GetInstance().FindFirst<GameStateEntity>();
        }

        private void HealthComponent_OnDeath(object sender, IDamageInfo info)
        {
            actor.AnimationHandler.SetAnyStateTransitionPriority(0, 3);
            actor.Animator.SetTrigger("Death");
            actor.SetBlockAllInput(true);
        }
    }
}
