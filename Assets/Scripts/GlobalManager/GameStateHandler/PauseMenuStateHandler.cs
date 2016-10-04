using UnityEngine;
using System.Collections;
using System;

namespace Manager
{
    public class PauseMenuStateHandler : MonoBehaviour, IGameState
    {
        public GameObject pauseMenuUIRoot;

        public GameStateType StateType
        {
            get
            {
                return GameStateType.Pause;
            }
        }

        public void OnStateActive()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameStateManager.GetInstance().EndCurrentState();
            }
        }

        public void OnStateStart()
        {
            pauseMenuUIRoot.SetActive(true);
            Time.timeScale = 0;
        }

        public void OnStateEnd()
        {
            pauseMenuUIRoot.SetActive(false);
            Time.timeScale = 1;
        }

        public override string ToString()
        {
            return "PauseState";
        }
    }
}
