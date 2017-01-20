using UnityEngine;
using Actors;
using System.Collections;
using Manager;

namespace Combat
{
    public class DeathHandler : MonoBehaviour
    {
        [AssignActorAutomaticly, SerializeField, HideInInspector]
        BasicEntityActor actor;

        GameStateActor gameStateActor;

        void Start()
        {
            actor.IHealth.OnDeath += HealthComponent_OnDeath;
            gameStateActor = ActorDatabase.GetInstance().FindFirst<GameStateActor>();
        }

        private void HealthComponent_OnDeath(object sender, IDamageInfo info)
        {
            actor.AnimationHandler.SetAnyStateTransitionPriority(0, 3);
            actor.Animator.SetTrigger("Death");
            actor.SetBlockAllInput(true);
        }
    }
}
