using UnityEngine;
using System.Collections;
using System;
using Entities;

namespace Manager
{
    public class GamePlayStateHandler : MonoBehaviour, IGameState
    {
        [AssignEntityAutomaticly, SerializeField, HideInInspector]
        GameStateEntity actor;


        void Start()
        {
            GameStateManager.Instance.AssignDefaultState(this);
        }

        public void OnStateActive()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameStateManager.Instance.StartNewState(actor.PauseMenuStateHandler);
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
