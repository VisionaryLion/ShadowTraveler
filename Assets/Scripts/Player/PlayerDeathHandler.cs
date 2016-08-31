using UnityEngine;
using Actors;
using System.Collections;
using Manager;

namespace Combat
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [AssignActorAutomaticly]
        PlayerActor actor;

        GameStateActor gameStateActor;

        void Start()
        {
            actor.BasicHealth.OnDeath += HealthComponent_OnDeath;
            gameStateActor = ActorDatabase.GetInstance().FindFirst<GameStateActor>();
        }

        private void HealthComponent_OnDeath(object sender, IDamageInfo info)
        {
            actor.CC2DMotor.frontAnimator.SetTrigger("Death");
            GameStateManager.GetInstance().StartNewState(gameStateActor.DeathStateHandler);
        }
    }
}
