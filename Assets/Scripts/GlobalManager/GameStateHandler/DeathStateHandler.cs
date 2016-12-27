using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Entity;

namespace Manager
{
    public class DeathStateHandler : MonoBehaviour, IGameState
    {
        [SerializeField]
        GameObject deathUIRoot;

        PlayerEntity actor;

        void Start()
        {
            actor = EntityDatabase.GetInstance().FindFirst<PlayerEntity>();
            actor.IHealth.OnDeath += IHealth_OnDeath;
        }

        private void IHealth_OnDeath(object sender, Combat.IDamageInfo e)
        {
            GameStateManager.Instance.StartNewState(this);
        }

        public void OnStateActive()
        {
            if (deathUIRoot.activeInHierarchy)
            {
                if (Input.anyKeyDown)
                {
                    GameStateManager.Instance.EndCurrentState();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                }
            }
        }

        public void OnStateStart()
        {
            //some death fx
            actor.AnimationHandler.StartListenToAnimationEnd("Death_Anim", new AnimationHandler.AnimationEvent(ActivateGameOverScreen));
            actor.AnimationHandler.SetAnyStateTransitionPriority(0, 3);
        }

        public void OnStateEnd()
        {
            deathUIRoot.SetActive(false);
            actor.SetBlockAllInput(false);
            actor.AnimationHandler.ResetAnyStateTransitionPriority(0);
        }

        void ActivateGameOverScreen()
        {
            deathUIRoot.SetActive(true);
        }

        public override string ToString()
        {
            return "DeathState";
        }
    }
}
