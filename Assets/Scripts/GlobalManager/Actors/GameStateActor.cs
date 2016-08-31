using UnityEngine;
using Manager;
using System.Collections;

namespace Actors
{
    [RequireComponent(typeof(DeathStateHandler))]
    [RequireComponent(typeof(PauseMenuStateHandler))]
    [RequireComponent(typeof(GamePlayStateHandler))]
    public class GameStateActor : Actor
    {
        [SerializeField]
        DeathStateHandler deathStateHandler;
        [SerializeField]
        PauseMenuStateHandler pauseMenuStateHandler;
        [SerializeField]
        GamePlayStateHandler gamePlayStateHandler;

        public DeathStateHandler DeathStateHandler { get { return deathStateHandler; } }
        public PauseMenuStateHandler PauseMenuStateHandler { get { return pauseMenuStateHandler; } }
        public GamePlayStateHandler GamePlayStateHandler { get { return gamePlayStateHandler; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            deathStateHandler = GetComponentInChildren<DeathStateHandler>();
            pauseMenuStateHandler = GetComponentInChildren<PauseMenuStateHandler>();
            gamePlayStateHandler = GetComponentInChildren<GamePlayStateHandler>();
        }
#endif
    }
}
