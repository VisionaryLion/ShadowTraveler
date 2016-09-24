using UnityEngine;
using System.Collections;
using System;
using Actors;

namespace Manager
{
    public class GamePlayStateHandler : MonoBehaviour, IGameState
    {
        [AssignActorAutomaticly]
        GameStateActor actor;

        public GameStateType StateType
        {
            get
            {
                return GameStateType.Play;
            }
        }

        void Start()
        {
            GameStateManager.GetInstance().AssignDefaultState(this);
        }

        public void OnStateActive()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameStateManager.GetInstance().StartNewState(actor.PauseMenuStateHandler);
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
