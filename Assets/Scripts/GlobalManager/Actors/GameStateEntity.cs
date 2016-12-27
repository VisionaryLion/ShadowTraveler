using UnityEngine;
using Manager;
using System.Collections;

namespace Entity
{
    [RequireComponent(typeof(DeathStateHandler))]
    [RequireComponent(typeof(PauseMenuStateHandler))]
    [RequireComponent(typeof(GamePlayStateHandler))]
    public class GameStateEntity : Entity
    {
        [SerializeField]
        DeathStateHandler deathStateHandler;
        [SerializeField]
        PauseMenuStateHandler pauseMenuStateHandler;
        [SerializeField]
        GamePlayStateHandler gamePlayStateHandler;

        GameStateManager gameStateManager;

        public DeathStateHandler DeathStateHandler { get { return deathStateHandler; } }
        public PauseMenuStateHandler PauseMenuStateHandler { get { return pauseMenuStateHandler; } }
        public GamePlayStateHandler GamePlayStateHandler { get { return gamePlayStateHandler; } }

        protected override void Awake()
        {
            base.Awake();
            gameStateManager = new GameStateManager();
        }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            deathStateHandler = LoadComponent<DeathStateHandler>(deathStateHandler);
            pauseMenuStateHandler = LoadComponent<PauseMenuStateHandler>(pauseMenuStateHandler);
            gamePlayStateHandler = LoadComponent<GamePlayStateHandler>(gamePlayStateHandler);
        }
#endif
    }
}
