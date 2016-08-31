using UnityEngine;
using System.Collections;
using System;

namespace Manager
{
    public class GamePlayStateHandler : MonoBehaviour, IGameState
    {
        public PauseMenuStateHandler pauseMenuStateHandler;
        public DeathStateHandler deathState;

        GameStateManager stateMan;

        void Awake()
        {
            stateMan = GameStateManager.GetInstance();
            stateMan.AssignDefaultState(this);
        }

        public void OnStateActive()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                stateMan.StartNewState(pauseMenuStateHandler);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                stateMan.StartNewState(deathState);
            }
        }

        public void OnStateStart()
        {
            //Do nothing, cause this is the default state. Every other state will revert to this state when it loses focus.
        }

        public void OnStateEnd()
        {
            //Do nothing, cause this is the default state. Every other state will revert to this state when it loses focus.
        }

        public override string ToString()
        {
            return "GamePlayState";
        }
    }
}
