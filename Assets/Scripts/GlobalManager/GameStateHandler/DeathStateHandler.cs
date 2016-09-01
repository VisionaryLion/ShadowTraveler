using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Actors;

namespace Manager
{
    public class DeathStateHandler : MonoBehaviour, IGameState
    {
        [SerializeField]
        GameObject deathUIRoot;

        PlayerActor actor;

        void Start()
        {
            actor = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
        }

        public void OnStateActive()
        {
            if (deathUIRoot.activeInHierarchy)
            {
                if (Input.anyKeyDown)
                {
                    GameStateManager.GetInstance().EndCurrentState();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                }
            }
        }

        public void OnStateStart()
        {
            //some death fx
            actor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.BlockMovement, PlayerLimitation.BlockNonMovement);
            actor.PlayerAnimationBaseLayerEnd.DeathAnimFinishedHandler += ActivateGameOverScreen;
        }

        public void OnStateEnd()
        {
            deathUIRoot.SetActive(false);
            actor.PlayerLimitationHandler.SetLimitation(PlayerLimitation.NoLimitation);
            actor.PlayerAnimationBaseLayerEnd.DeathAnimFinishedHandler -= ActivateGameOverScreen;
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
